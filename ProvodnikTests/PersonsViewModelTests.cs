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

        private void SetUpMocks(IQueryable<Person> persons, IQueryable<PersonDoc> personDocs)
        {

            var personsMockSet = new Mock<DbSet<Person>>();
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(persons.Provider);
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(persons.Expression);
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(persons.ElementType);
            personsMockSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(persons.GetEnumerator());

            var personDocsMockSet = new Mock<DbSet<PersonDoc>>();
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.Provider).Returns(personDocs.Provider);
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.Expression).Returns(personDocs.Expression);
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.ElementType).Returns(personDocs.ElementType);
            personDocsMockSet.As<IQueryable<PersonDoc>>().Setup(m => m.GetEnumerator()).Returns(personDocs.GetEnumerator());

            var mockContext = new Mock<ProvodnikContext>();
            mockContext.Setup(c => c.Persons).Returns(personsMockSet.Object);
            mockContext.Setup(c => c.PersonDocs).Returns(personDocsMockSet.Object);

            NinjectContext.Kernel.Rebind<ProvodnikContext>().ToConstant(mockContext.Object);
        }

        [Test()]
        public void RefreshPersonListTest()
        {
            #region Fill db
            var persons = new List<Person>
            {
                new Person { Id=1,Pol= "мужской" },
                new Person { Id=2,Pol= "мужской" },
                new Person { Id=3,Pol= "мужской" },
                new Person { Id=4},//no docs
            }.AsQueryable();

            var personDocs = new List<PersonDoc>
            {
                new PersonDoc { PersonId=1,DocTypeId=DocConsts.ВоенныйБилет,FileName="file"},
                new PersonDoc { PersonId=1,DocTypeId=DocConsts.Паспорт,FileName="file"},
                new PersonDoc { PersonId=1,DocTypeId=DocConsts.Прописка,FileName="file"},

                new PersonDoc { PersonId=2,DocTypeId=DocConsts.Паспорт,FileName="file"},
                new PersonDoc { PersonId=2,DocTypeId=DocConsts.Прописка},
                new PersonDoc { PersonId=2,DocTypeId=DocConsts.ВоенныйБилет},

                new PersonDoc { PersonId=3,DocTypeId=DocConsts.Приписное1,FileName="file"},
                new PersonDoc { PersonId=3,DocTypeId=DocConsts.Приписное2,FileName="file"},
                new PersonDoc { PersonId=3,DocTypeId=DocConsts.Паспорт,FileName="file"},
                new PersonDoc { PersonId=3,DocTypeId=DocConsts.Прописка,FileName="file"},
            }.AsQueryable();

            SetUpMocks(persons, personDocs);
            #endregion

            var vm = new PersonsViewModel();
            //var db=NinjectContext.Get<ProvodnikContext>();            //var docs = db.Persons.FirstOrDefault();//.PersonDocs;            //var docsl = db.Persons.ToList();

            vm.RefreshPersonList();
            Assert.That(vm.PersonList.Count == 4);

            vm.ExtendedChecks.First(pp => pp.DocType == DocConsts.ВоенныйБилет).IsChecked = true;
            vm.RefreshPersonList();
            var ids = vm.PersonList.Select(pp => pp.Id).ToArray();
            CollectionAssert.AreEquivalent(ids, new int[] { 1, 3 });

            vm.ExtendedChecks.First(pp => pp.DocType == DocConsts.ВоенныйБилет).IsChecked = false;
            vm.RefreshPersonList();
             ids = vm.PersonList.Select(pp => pp.Id).ToArray();
            CollectionAssert.AreEquivalent(ids, new int[] { 2 });

            //Assert.That(docs.Count ==2);
            //Assert.Fail();
        }
    }
}