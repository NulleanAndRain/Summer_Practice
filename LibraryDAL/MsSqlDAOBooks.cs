using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace LibraryDAL {
    public class MsSqlDAOBooks : MsSqlDAOBase, IDAOBooks {

        public MsSqlDAOBooks(string ConnString) : base(ConnString) { }

        #region Books

        public int AddBook(Book book) {
            var querry = @"INSERT INTO Books (BookName, Authors, YearOfPublishing)
                           VALUES (@n, @a, @y)
                           SELECT SCOPE_IDENTITY()";
            var cmd = new SqlCommand(querry, conn);
            cmd.Parameters.AddRange(new SqlParameter[] {
                new SqlParameter("@n", book.Name),
                new SqlParameter("@a", book.Authors),
                new SqlParameter("@y", book.YearOfPublishing)
            });
            try {
                return Convert.ToInt32(cmd.ExecuteScalar());
            } catch {
                return -1;
            }
        }

        private List<Book> getBooksWithQuerry(string querrySelector = "", List<SqlParameter> _params = null) {
            List<Book> books = new List<Book>();
            var querry = "SELECT ID, BookName, Authors, YearOfPublishing FROM Books ";
            var cmd = new SqlCommand(querry + querrySelector, conn);
            if (_params != null) {
                foreach (var param in _params) {
                    cmd.Parameters.Add(param);
                }
            }
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    var book = new Book();
                    book.Id = reader.GetInt32(0);
                    book.Name = reader.GetString(1);
                    book.Authors = reader.GetString(2);
                    book.YearOfPublishing = reader.GetInt32(3);
                    books.Add(book);
                }
            }
            return books;
        }

        public List<Book> GetBooks() {
            return getBooksWithQuerry();
        }

        public List<Book> GetBooksWithAuthors(string[] authors) {
            if (authors.Length == 0) return null;
            var querrySelector = new StringBuilder("WHERE Authors LIKE ");
            var _params = new List<SqlParameter>();
            for (int i = 0; i < authors.Length; i++) {
                var author = authors[i];
                var paramName = $"@a{i}";
                querrySelector.Append(paramName);
                _params.Add(new SqlParameter(paramName, $"%{author}%"));
                if (i != authors.Length - 1) {
                    querrySelector.Append("AND LIKE ");
                }
            }
            return getBooksWithQuerry(querrySelector.ToString(), _params);
        }

        public List<Book> GetBooksWithName(string name) {
            var querrySelector = "WHERE BookName LIKE @n";
            var p = new List<SqlParameter> { new SqlParameter("@n", $"%{name}%") };
            return getBooksWithQuerry(querrySelector, p);
        }

        public void EditBook(int id, Book newData) {
            var querry = @"UPDATE Books
                SET BookName = @n, Authors = @a, YearOfPublishing = @y
                WHERE ID = @id";
            var p = new List<SqlParameter> {
                new SqlParameter("@n", newData.Name),
                new SqlParameter("@a", newData.Authors),
                new SqlParameter("@y", newData.YearOfPublishing),
                new SqlParameter("@id", id)
            };
            execNonQuerry(querry, p);
        }

        public void DeleteBook(int id) {
            var querry = @"DELETE FROM Books WHERE ID = @id";
            var p = new List<SqlParameter> { new SqlParameter("@id", id) };
            execNonQuerry(querry, p);
        }

        public void UpdateBookPicture(int id, BitmapImage img) {
            var querry = @"UPDATE Books SET PreviewImg = @img WHERE ID = @id";
            var p = new List<SqlParameter> {
                new SqlParameter("@img", ImgReader.GetBytes(img)),
                new SqlParameter("@id", id)
            };
            execNonQuerry(querry, p);
        }

        public BitmapImage GetBookPicture(int id) {
            var querry = @"SELECT PreviewImg FROM Books WHERE ID = @id";
            var cmd = new SqlCommand(querry, conn);
            cmd.Parameters.Add(new SqlParameter("@id", id));
            using (var reader = cmd.ExecuteReader()) {
                BitmapImage img = null;
                if (reader.Read()) {
                    img = ImgReader.GetImage(reader.GetStream(0));
                }
                return img;
            }
        }

        public void UpdateBookFile(int id, byte[] file, string filename) {
            var querry = @"UPDATE Books SET AttachedFile = @f, FileName = @n WHERE ID = @id";
            if (file == null) file = new byte[0];
            var p = new List<SqlParameter> {
                new SqlParameter("@f", file),
                new SqlParameter("@n", filename),
                new SqlParameter("@id", id)
            };
            execNonQuerry(querry, p);
        }

        public void GetBookFile(int id, Action<byte[], string> onFileLoad) {
            var querry = @"SELECT AttachedFile, FileName FROM Books WHERE ID = @id";
            var cmd = new SqlCommand(querry, conn);
            cmd.Parameters.Add(new SqlParameter("@id", id));
            using (var reader = cmd.ExecuteReader()) {
                byte[] file = new byte[0];
                string name = string.Empty;
                try {
                    if (reader.Read()) {
                        var ms = new MemoryStream();
                        reader.GetStream(0).CopyTo(ms);
                        file = ms.ToArray();
                        name = reader.GetString(1);
                    }
                } catch { }
                onFileLoad(file, name);
            }
        }

        #endregion
    }
}
