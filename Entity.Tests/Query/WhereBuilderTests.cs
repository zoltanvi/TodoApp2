using Entity.Tests.Mocks;
using System;
using TodoApp2.Entity.Query;
using Xunit;

namespace Entity.Tests.Query
{
    public class WhereBuilderTests
    {
        [Fact]
        public void EqualsExpressionTest1()
        {
            var expected = "((Trashed = False) AND ((Trashed != False) = False))";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => (x.Trashed == false) && !(x.Trashed != false));
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest2()
        {
            var expected = "((Trashed != True) AND (False = Trashed))";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => x.Trashed != true && false == x.Trashed);
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest3_DoesNotResolveModelPropertyValue()
        {
            var expected = "(ModificationDate = CreationDate)";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => x.ModificationDate == ((x.CreationDate)));
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest4()
        {
            var expected = "(((Title != 'a') AND (Title != '1')) AND (Title = 'qwe'))";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => x.Title != "a" && x.Title != "1" && x.Title == "qwe");
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest5()
        {
            var expected = "((Trashed = FALSE) OR Trashed)";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => !x.Trashed || x.Trashed);
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest6()
        {
            var expected = "(Trashed = FALSE)";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => !x.Trashed);
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest7()
        {
            var expected = "(Trashed AND Trashed)";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => x.Trashed && x.Trashed);
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest8_ResolvesNonModelPropertyValue()
        {
            var vmMock = new NoteVMMock() { Id = 42 };

            var expected = "(Id = 42)";
            var actual = WhereBuilder.ExpressionToString<NoteMock>(x => x.Id == vmMock.Id);
            Assert.Equal(expected, actual, true);
        }

        [Fact]
        public void EqualsExpressionTest_Exception1()
        {
            bool ex = false;
            try
            {
                WhereBuilder.ExpressionToString<NoteMock>(x => x.IsGood());
            }
            catch (ArgumentException)
            {
                ex = true;
            }

            Assert.True(ex);
        }

        [Fact]
        public void EqualsExpressionTest_Exception2()
        {
            bool ex = false;
            try
            {
               WhereBuilder.ExpressionToString<NoteMock>(x => x);
            }
            catch (ArgumentException)
            {
                ex = true;
            }

            Assert.True(ex);
        }
    }
}