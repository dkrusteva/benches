using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using WalksAndBenches.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace WalksAndBenches.UnitTests.Controllers
{
    public class AppControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IndexAction_ReutrnsIndexView()
        {
            var controller = new AppController();
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName); 
        }

        [Test]
        public void AboutAction_ReutrnsAboutView()
        {
            var controller = new AppController();
            var result = controller.About() as ViewResult;
            Assert.AreEqual("About", result.ViewName);
        }
    }
}
