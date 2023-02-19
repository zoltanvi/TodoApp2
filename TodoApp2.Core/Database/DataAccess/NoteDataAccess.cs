using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    // CRUD = Create, Read, Update, Delete
    public class NoteDataAccess : BaseDataAccess
    {
        public NoteDataAccess(SQLiteConnection connection) : base(connection)
        {
        }

        // Create

        public void CreateNoteTable()
        {
            string createNoteTable = 
                $"CREATE TABLE IF NOT EXISTS {Table.Note} (" +
                $" {Column.Id} INTEGER PRIMARY KEY, " +
                $" {Column.Title} TEXT, " +
                $" {Column.ListOrder} TEXT DEFAULT ('{DefaultListOrder}'), " +
                $" {Column.Content} TEXT, " +
                $" {Column.CreationDate} INTEGER, " +
                $" {Column.ModificationDate} INTEGER, " +
                $" {Column.Trashed} INTEGER DEFAULT (0)" +
                $"); ";

            ExecuteCommand(createNoteTable);
        }

        public NoteViewModel CreateNote(string noteTitle)
        {
            NoteViewModel note = new NoteViewModel
            {
                Id = GetNextId(),
                Title = noteTitle,
                Content = string.Empty,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks,
                Trashed = false,
                ListOrder = GetListOrder(Table.Note, true) - ListOrderInterval
            };

            AddNote(note);

            return note;
        }

        // Read
        public List<NoteViewModel> GetActiveNotes()
        {
            string query =
                $"SELECT * FROM {Table.Note} " +
                $" WHERE {Column.Trashed} = 0 " +
                $" ORDER BY {Column.ListOrder}";

            return GetItemsWithQuery(query, ReadNote);
        }

        // Update

        public bool UpdateNote(NoteViewModel note)
        {
            bool result = false;
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"UPDATE {Table.Note} SET " +
                                      $"  {Column.Title} = {Parameter.Title}, " +
                                      $"  {Column.ListOrder} = {Parameter.ListOrder}, " +
                                      $"  {Column.Content} = {Parameter.Content}, " +
                                      $"  {Column.CreationDate} = {Parameter.CreationDate}, " +
                                      $"  {Column.ModificationDate} = {Parameter.ModificationDate}, " +
                                      $"  {Column.Trashed} = {Parameter.Trashed} " +
                                      $" WHERE {Column.Id} = {Parameter.Id};";
                command.Parameters.AddRange(CreateNoteParameterList(note));

                result = command.ExecuteNonQuery() > 0;
            }

            return result;
        }

        private void AddNote(NoteViewModel note)
        {
            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"INSERT INTO {Table.Note} " +
                                      $" ({Column.Id}, {Column.Title}, {Column.ListOrder}, " +
                                      $" {Column.Content}, {Column.CreationDate}, {Column.ModificationDate}, " +
                                      $" {Column.Trashed}) " +
                                      $" VALUES ({Parameter.Id}, {Parameter.Title}, {Parameter.ListOrder}, " +
                                      $" {Parameter.Content}, {Parameter.CreationDate}, {Parameter.ModificationDate}, " +
                                      $" {Parameter.Trashed});";

                command.Parameters.AddRange(CreateNoteParameterList(note));

                command.ExecuteNonQuery();
            }
        }

        private SQLiteParameter[] CreateNoteParameterList(NoteViewModel note)
        {
            return new[]
            {
                new SQLiteParameter(Parameter.Id, note.Id),
                new SQLiteParameter(Parameter.Title, note.Title),
                new SQLiteParameter(Parameter.ListOrder, note.ListOrder.ToString("D19")),
                new SQLiteParameter(Parameter.Content, note.Content),
                new SQLiteParameter(Parameter.CreationDate, note.CreationDate),
                new SQLiteParameter(Parameter.ModificationDate, note.ModificationDate),
                new SQLiteParameter(Parameter.Trashed, note.Trashed)
            };
        }

        private NoteViewModel ReadNote(SQLiteDataReader reader)
        {
            NoteViewModel note = new NoteViewModel
            {
                Id = reader.SafeGetInt(Column.Id),
                Title = reader.SafeGetString(Column.Title),
                ListOrder = reader.SafeGetLongFromString(Column.ListOrder),
                Content = reader.SafeGetString(Column.Content),
                CreationDate = reader.SafeGetLong(Column.CreationDate),
                ModificationDate = reader.SafeGetLong(Column.ModificationDate),
                Trashed = reader.SafeGetBoolFromInt(Column.Trashed)
            };

            return note;
        }

        private int GetNextId()
        {
            int nextId = 0;

            using (SQLiteCommand command = new SQLiteCommand(m_Connection))
            {
                command.CommandText = $"SELECT * FROM {Table.Note} ORDER BY {Column.Id} DESC LIMIT 1";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextId = reader.SafeGetInt(Column.Id) + 1;
                        break;
                    }
                }
            }

            return nextId;
        }
    }
}
