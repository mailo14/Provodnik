using Moq;
using NUnit.Framework;
using Provodnik;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik.Tests
{
    [TestFixture()]
    public class PersonsViewModelTests
    {
        [Test()]
        public void RefreshPersonListTest()
        {
            var persons = new List<Person>
            {
                new Person { Fio = "BBB",Id=1 },
            }.AsQueryable();

            var personsMockSet = new Mock<DbSet<Person>>();
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(persons.Provider);
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(persons.Expression);
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(persons.ElementType);
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(persons.GetEnumerator());

            var personDocs = new List<PersonDoc>
            {
                new PersonDoc {  Id=1,PersonId=1,DocTypeId=DocConsts.ВоенныйБилет},
                new PersonDoc {  Id=2,PersonId=1,DocTypeId=DocConsts.Паспорт},
            }.AsQueryable();

            var personDocsMockSet = new Mock<DbSet<PersonDoc>>();
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.Provider).Returns(personDocs.Provider);
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.Expression).Returns(personDocs.Expression);
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.ElementType).Returns(personDocs.ElementType);
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.GetEnumerator()).Returns(personDocs.GetEnumerator());

            var mockContext = new Mock<ProvodnikContext>();
            mockContext.Setup(c => c.Persons).Returns(personsMockSet.Object);
            mockContext.Setup(c => c.PersonDocs).Returns(personDocsMockSet.Object);


            var vm = new PersonsViewModel();// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
            vm.RefreshPersonList();
            //var c = vm.PersonList.Count > 0;
            Assert.That(vm.PersonList.Count ==1);
            //Assert.Fail();
        }
    }
}