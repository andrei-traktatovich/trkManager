using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TicketDataModel;
using System.Data.Entity;
using BusinessLogic;
using System.Data;
using System.Linq;

namespace BusinessLogic.Tests
{
    [TestClass]
    public class DomainManagerShould
    {
        [TestMethod]
        public void should_return_false_if_translator_already_has_this_domain_in_this_direction()
        {
            var ctx = new TraktatEntities();
            var dm = new DomainManager(ctx);
            var result = dm.CheckTranslatorDomain(638, 2249, 259);
            Assert.AreEqual(false, result.Success);
        }
        [TestMethod]
        public void should_return_true_if_translator_does_not_have_this_domain_in_this_direction()
        {
            var ctx = new TraktatEntities();
            var dm = new DomainManager(ctx);
            var result = dm.CheckTranslatorDomain(638, 2249, -1);
            Assert.AreEqual(true, result.Success);
        }

        
        [TestMethod]
        public void should_return_false_if_translator_does_not_have_this_direction()
        {
            var ctx = new TraktatEntities();
            var dm = new DomainManager(ctx);
            var result = dm.CheckTranslatorDomain(638, -1, -1);
            Assert.AreEqual(false, result.Success);
        }

        [TestMethod]
        public void should_return_false_if_translator_does_not_exist()
        {
            var ctx = new TraktatEntities();
            var dm = new DomainManager(ctx);
            var result = dm.CheckTranslatorDomain(-1, -1, -1);
            Assert.AreEqual(false, result.Success);
        }

        [TestMethod]
        public void should_update_existing_domain()
        {
            var ctx = new TraktatEntities();
            var dm = new DomainManager(ctx);
            var result = dm.AddOrUpdateTranslatorDomain(638, 2249, 16, 638, 2, 9);
            Assert.AreEqual("Тематика обновлена", result.ErrorMessage);
        }

        [TestMethod]
        public void should_insert_new_domain()
        {
            var ctx = new TraktatEntities();
            var dm = new DomainManager(ctx);
            var result = dm.AddOrUpdateTranslatorDomain(638, 2249, 16, 638, 2, 7);
            Assert.AreEqual("Тематика добавлена", result.ErrorMessage);
        }
        
        [TestMethod]
        public void when_checking_domain_name_for_uniqueness_return_true_if_there_is_no_match()
        {
            var ctx = new TraktatEntities();
            var name = " догоВор несуществующее название тематики    ";
            var id = -1;
            var result = (new DomainManager(ctx).IsNameUnique(id, name));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void when_checking_domain_name_for_uniqueness_return_false_if_same_id_and_name()
        {
            var ctx = new TraktatEntities();
            var domain = ctx.Domains.OrderBy(x => x.ID).Skip(3).First();
            var name = domain.Name;
            var id = domain.ID;
            var result = (new DomainManager(ctx).IsNameUnique(id, name));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void when_checking_domain_name_for_uniqueness_return_true_if_no_match()
        {
            var ctx = new TraktatEntities();
            var domain = ctx.Domains.OrderBy(x => x.ID).Skip(3).First();
            var name = domain.Name;
            var id = domain.ID + 1;
            var result = (new DomainManager(ctx).IsNameUnique(id, name));
            Assert.AreEqual(false, result);
        }
    }
}
