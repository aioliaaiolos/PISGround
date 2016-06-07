using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIS.Ground.InstantMessage;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TemplateListAccessorClassUnitTest
    /// </summary>
    [TestClass]
    public class TemplateListAccessorClassUnitTest
    {
        public TemplateListAccessorClassUnitTest()
        {            
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestWriteTemplatesToFile()
        {
            List<Template> x = new List<Template>();
            {
                Template t = new Template();
                {
                    t.ID = "X123";
                    t.Class = "TEST CLASS";
                    t.Category = TemplateCategory.Scheduled;
                    t.DescriptionList = new List<TemplateDescription>();
                    {
                        TemplateDescription c = new TemplateDescription();
                        {
                            c.Language = "fra";
                            c.Value = "Test description fra";
                        }
                        t.DescriptionList.Add(c);
                    }
                    {
                        TemplateDescription c = new TemplateDescription();
                        {
                            c.Language = "eng";
                            c.Value = "Test description eng";
                        }
                        t.DescriptionList.Add(c);
                    }
                    t.ParameterList = new List<TemplateParameterType>();
                    {
                        t.ParameterList.Add(TemplateParameterType.CarNumber);
                        t.ParameterList.Add(TemplateParameterType.Delay);
                        t.ParameterList.Add(TemplateParameterType.DelayReasonCode);
                        t.ParameterList.Add(TemplateParameterType.StationId);
                        t.ParameterList.Add(TemplateParameterType.Text);
                    }
                }
                x.Add(t);
            }

            TemplateListAccessor.WriteTemplatesToFile(x, @"c:\test.xml");            
        }

        [TestMethod]
        public void TestReadTemplatesFromFile()
        {
            List<Template> y = TemplateListAccessor.ReadTemplatesFromFile(@"c:\test.xml");
        }

        [TestMethod]
        public void TestTemplateExists()
        {
            TemplateListAccessor obj = new TemplateListAccessor();
            obj.ExecuteTemplate(@"c:\test.xml");

            Assert.IsTrue(obj.TemplateExists("X123"));
            Assert.IsFalse(obj.TemplateExists("ABCD"));
        }

        [TestMethod]
        public void TestGetTemplate()
        {
            TemplateListAccessor obj = new TemplateListAccessor();
            obj.ExecuteTemplate(@"c:\test.xml");

            Assert.IsNotNull(obj.GetTemplate("X123"));
            Assert.IsNull(obj.GetTemplate("ABCD"));
        }
    }
}
