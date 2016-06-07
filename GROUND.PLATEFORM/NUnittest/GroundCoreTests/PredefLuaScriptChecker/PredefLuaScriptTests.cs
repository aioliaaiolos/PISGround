using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

using PIS.Ground.InstantMessage;
using System.IO;
using System.Reflection;

namespace GroundCoreTests
{
    /// <summary>PredefLuaScriptTests test class.</summary>
    [TestFixture]
    public class PredefLuaScriptTests
    {
        #region attributes

        #endregion

        #region Tests managment

        /// <summary>Initializes a new instance of the PredefLuaScriptTests class.</summary>
        public PredefLuaScriptTests()
        {
        }

        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
            // Do something after each tests
        }

        #endregion

        #region PredefLuaScriptTests

        /// <summary>Test for predef lua script. Reading lua scripts to read predefined messages</summary>
        [Test]
        public void TestPredefLuaScript1()
        {
            string strExecutionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty);

            TemplateListAccessor templateAccessor = new TemplateListAccessor();
            Assert.AreEqual(templateAccessor.ExecuteTemplate(strExecutionPath + "\\..\\..\\PredefLuaScriptChecker\\Predef1.lua", new List<string>()), true);
            List<Template> templateList = templateAccessor.GetAllTemplates();

            Assert.AreEqual(templateList.ElementAt(0).ID, "100");
            Assert.AreEqual(templateList.ElementAt(0).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(0).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(1).ID, "101");
            Assert.AreEqual(templateList.ElementAt(1).Class, "PRIORITAIRE");
            Assert.AreEqual(templateList.ElementAt(1).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(2).ID, "102");
            Assert.AreEqual(templateList.ElementAt(2).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(2).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(3).ID, "103");
            Assert.AreEqual(templateList.ElementAt(3).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(3).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(4).ID, "104");
            Assert.AreEqual(templateList.ElementAt(4).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(4).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(5).ID, "105");
            Assert.AreEqual(templateList.ElementAt(5).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(5).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(6).ID, "106");
            Assert.AreEqual(templateList.ElementAt(6).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(6).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(7).ID, "107");
            Assert.AreEqual(templateList.ElementAt(7).Class, "PRIORITAIRE");
            Assert.AreEqual(templateList.ElementAt(7).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(8).ID, "108");
            Assert.AreEqual(templateList.ElementAt(8).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(8).Category, TemplateCategory.FreeText);

            Assert.AreEqual(templateList.ElementAt(9).ID, "109");
            Assert.AreEqual(templateList.ElementAt(9).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(9).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(10).ID, "111");
            Assert.AreEqual(templateList.ElementAt(10).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(10).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(11).ID, "110");
            Assert.AreEqual(templateList.ElementAt(11).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(11).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(12).ID, "10A");
            Assert.AreEqual(templateList.ElementAt(12).Class, "PRIORITAIRE");
            Assert.AreEqual(templateList.ElementAt(12).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(13).ID, "200");
            Assert.AreEqual(templateList.ElementAt(13).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(13).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(14).ID, "201");
            Assert.AreEqual(templateList.ElementAt(14).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(14).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(15).ID, "202");
            Assert.AreEqual(templateList.ElementAt(15).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(15).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(16).ID, "204");
            Assert.AreEqual(templateList.ElementAt(16).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(16).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(17).ID, "205");
            Assert.AreEqual(templateList.ElementAt(17).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(17).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(18).ID, "206");
            Assert.AreEqual(templateList.ElementAt(18).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(18).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(19).ID, "207");
            Assert.AreEqual(templateList.ElementAt(19).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(19).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(20).ID, "208");
            Assert.AreEqual(templateList.ElementAt(20).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(20).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(21).ID, "209");
            Assert.AreEqual(templateList.ElementAt(21).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(21).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(22).ID, "211");
            Assert.AreEqual(templateList.ElementAt(22).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(22).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(23).ID, "212");
            Assert.AreEqual(templateList.ElementAt(23).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(23).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(24).ID, "213");
            Assert.AreEqual(templateList.ElementAt(24).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(24).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(25).ID, "214");
            Assert.AreEqual(templateList.ElementAt(25).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(25).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(26).ID, "215");
            Assert.AreEqual(templateList.ElementAt(26).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(26).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(27).ID, "216");
            Assert.AreEqual(templateList.ElementAt(27).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(27).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(28).ID, "217");
            Assert.AreEqual(templateList.ElementAt(28).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(28).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(29).ID, "218");
            Assert.AreEqual(templateList.ElementAt(29).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(29).Category, TemplateCategory.Predefined);

        }

        /// <summary>Test for predef lua script. Reading lua scripts to read predefined messages</summary>
        [Test]
        public void TestPredefLuaScript2()
        {
            string strExecutionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty);

            TemplateListAccessor templateAccessor = new TemplateListAccessor();
            Assert.AreEqual(templateAccessor.ExecuteTemplate(strExecutionPath + "\\..\\..\\PredefLuaScriptChecker\\Predef2.lua", new List<string>()), true);
            List<Template> templateList = templateAccessor.GetAllTemplates();

            Assert.AreEqual(templateList.ElementAt(0).ID, "1");
            Assert.AreEqual(templateList.ElementAt(0).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(0).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(1).ID, "2");
            Assert.AreEqual(templateList.ElementAt(1).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(1).Category, TemplateCategory.Predefined);

            Assert.AreEqual(templateList.ElementAt(2).ID, "T001");
            Assert.AreEqual(templateList.ElementAt(2).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(2).Category, TemplateCategory.FreeText);

            Assert.AreEqual(templateList.ElementAt(3).ID, "D001");
            Assert.AreEqual(templateList.ElementAt(3).Class, "INFO");
            Assert.AreEqual(templateList.ElementAt(3).Category, TemplateCategory.Predefined);

        }

        #endregion
    }
}
