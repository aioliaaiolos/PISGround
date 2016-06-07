using System;
//---------------------------------------------------------------------------------------------------
// <copyright file="LmtDatabaseAccessorTests.cs" company="Alstom">
//          (c) Copyright ALSTOM 2014.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using PIS.Ground.Core;
using PIS.Ground.Core.PackageAccess;

namespace GroundCoreTests
{
	/// <summary>HistoryLoggerTests test class.</summary>
	[TestFixture]
	public class LmtDatabaseAccessorTests
	{
		#region attributes

		/// <summary>The Database for URBAN tests.</summary>
		private string _urbanDB = "LMTURBAN.db";

		/// <summary>The Database for SIVENG test.</summary>
		private string _sivengDB = "LMT2N2.db";

        /// <summary>The Database for SIVENG / PP / Regiolis test.</summary>
        private string _sivengRegiolisDB = "LMTSIVE_Regiolis.db";

		/// <summary>Full pathname of the execution folder.</summary>
		private string _executionPath = string.Empty;

		/// <summary>The full path to Database for URBAN tests.</summary>
		private string _urbanDBPath = string.Empty;

		/// <summary>The full path to Database for SIVENG test.</summary>
		private string _sivengDBPath = string.Empty;

        /// <summary>The full path to Database for SIVENG / PP / Regiolis test.</summary>
        private string _sivengRegiolisDBPath = string.Empty;

		#endregion

		#region Tests managment

		/// <summary>Initializes a new instance of the LmtDatabaseAccessorTests class.</summary>
		public LmtDatabaseAccessorTests()
		{
		}

		/// <summary>Setups called before each test to initialize variables.</summary>
		[SetUp]
		public void Setup()
		{
			_executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty) + "\\..\\..\\" + "PackageAccess\\";
			_urbanDBPath = _executionPath + _urbanDB;
			_sivengDBPath = _executionPath + _sivengDB;
            _sivengRegiolisDBPath = _executionPath + _sivengRegiolisDB;
		}

		/// <summary>Tear down called after each test to clean.</summary>
		[TearDown]
		public void TearDown()
		{
			// Do something after each tests
		}

		#endregion

		#region LmtDatabaseAccessorTests - SIVENG

		/// <summary>Test StationExists with true statement expected.</summary>
		[Test]
		public void SivengStationExistsTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				string stationOperatorCode = "83003400";

				ExecuteStationExistsTrue(accessor, stationOperatorCode);
			}
		}

		/// <summary>Test StationExists with false statement expected.</summary>
		[Test]
		public void SivengStationExistsFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteStationExistsFalse(accessor);
			}
		}

		/// <summary>Test GetMissionInternalCodeFromOperatorCode with valid result expected.</summary>
		[Test]
		public void SivengGetMissionInternalCodeFromOperatorCodeValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				string operatorCode = "TGV002065AA";
				uint? expectedResult = 1066941;

				ExecuteGetMissionInternalCodeFromOperatorCodeValid(accessor, operatorCode, expectedResult);
			}
		}

		/// <summary>Test GetMissionInternalCodeFromOperatorCode with no valid result expected.</summary>
		[Test]
		public void SivengGetMissionInternalCodeFromOperatorCodeNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteGetMissionInternalCodeFromOperatorCodeNotValid(accessor);
			}
		}

		/// <summary>Test LanguageCodesExist with true statement expected.</summary>
		[Test]
		public void SivengLanguageCodesExistsTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				List<string> languagesIdList = new List<string>();
				languagesIdList.Add("fra");
				languagesIdList.Add("eng");
				languagesIdList.Add("ger");
				languagesIdList.Add("ita");
				languagesIdList.Add("spa");
				languagesIdList.Add("cat");

				ExecuteLanguageCodesExistsTrue(accessor, languagesIdList);
			}
		}

		/// <summary>Test LanguageCodesExist with false statement expected.</summary>
		[Test]
		public void SivengLanguageCodesExistsFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
                ExecuteLanguageCodesExistsFalse(accessor);
			}
		}

		/// <summary>Test ServiceCodesExists with true statement expected.</summary>
		[Test]
		public void SivengServiceCodesExistTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
                List<uint> onboardServiceCodeList = new List<uint>();
                onboardServiceCodeList.Add(0);
                onboardServiceCodeList.Add(4);
                onboardServiceCodeList.Add(9);
                onboardServiceCodeList.Add(15);

                ExecuteServiceCodesExistsTrue(accessor, onboardServiceCodeList);
			}
		}

		/// <summary>Test ServiceCodesExists with true statement expected.</summary>
		[Test]
		public void SivengServiceCodesExistFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteServiceCodesExistsFalse(accessor);
			}
		}

		/// <summary>Test RegionCodeExist with true statement expected.</summary>
		[Test]
		public void SivengRegionCodeExistTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteRegionCodeExistTrue(accessor);
			}
		}

		/// <summary>Test RegionCodeExist with false statement expected.</summary>
		[Test]
		public void SivengRegionCodeExistFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteRegionCodeExistFalse(accessor);
			}
		}

		/// <summary>Test GetStationOperatorCodeFromInternalCode with true statement expected.</summary>
		[Test]
		public void SivengGetStationOperatorCodeFromInternalCodeValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				string stationOperatorCodeExpected = "87743716";

				ExecuteGetStationOperatorCodeFromInternalCodeValid(accessor, stationOperatorCodeExpected);
			}
		}

		/// <summary>Test GetStationOperatorCodeFromInternalCode with false statement expected.</summary>
		[Test]
		public void SivengGetStationOperatorCodeFromInternalCodeNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteGetStationOperatorCodeFromInternalCodeNotValid(accessor);
			}
		}
		
		/// <summary>Test GetStationInternalCodeFromOperatorCode with true statement expected.</summary>
		[Test]
		public void SivengGetStationInternalCodeFromOperatorCodeValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				uint? stationInternalCodeExpected = 7;
				string stationOperatorCode = "87743716";

				ExecuteGetStationInternalCodeFromOperatorCodeValid(accessor, stationOperatorCode, stationInternalCodeExpected);
			}
		}

		/// <summary>Test GetStationInternalCodeFromOperatorCode with false statement expected.</summary>
		[Test]
		public void SivengGetStationInternalCodeFromOperatorCodeNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteGetStationInternalCodeFromOperatorCodeNotValid(accessor);
			}
		}
		
		/// <summary>Test RegionCodeExist with true statement expected.</summary>
		[Test]
		public void SivengGetMissionRouteValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				uint missionId = 1068167;
				List<uint> missionRouteExpected = new List<uint>();
				missionRouteExpected.Add(139);
				missionRouteExpected.Add(143);
				missionRouteExpected.Add(183);
				missionRouteExpected.Add(175);

				ExecuteGetMissionRouteValid(accessor, missionId, missionRouteExpected);
			}
		}

		/// <summary>Test RegionCodeExist with false statement expected.</summary>
		[Test]
		public void SivengGetMissionRouteNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				ExecuteGetMissionRouteNotValid(accessor);
			}
		}

		/// <summary>Test GetAllLanguages with results expected.</summary>
		[Test]
		public void SivengGetAllLanguagesTests()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				List<string> languagesExpected = new List<string>();
				languagesExpected.Add("fra");
				languagesExpected.Add("eng");
				languagesExpected.Add("ger");
				languagesExpected.Add("ita");
				languagesExpected.Add("spa");
				languagesExpected.Add("cat");

				ExecuteGetAllLanguagesTests(accessor, languagesExpected);
			}
		}

		/// <summary>Test GetStationList with results expected.</summary>
		[Test]
		public void SivengGetStationListTest()
		{
			using (var accessor = new LmtDatabaseAccessor(_sivengDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
			{
				int numberOfStationExpected = 301;

				ExecuteGetStationListTest(accessor, numberOfStationExpected);
			}
		}

		#endregion

        #region LmtDatabaseAccessorTests - SIVENG / PP / Regiolis

        /// <summary>Test StationExists with true statement expected.</summary>
        [Test]
        public void SivengRegiolisStationExistsTrue()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                string stationOperatorCode = "0087672253BV";

                ExecuteStationExistsTrue(accessor, stationOperatorCode);
            }
        }

        /// <summary>Test StationExists with false statement expected.</summary>
        [Test]
        public void SivengRegiolisStationExistsFalse()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteStationExistsFalse(accessor);
            }
        }

        /// <summary>Test GetMissionInternalCodeFromOperatorCode with valid result expected.</summary>
        [Test]
        public void SivengRegiolisGetMissionInternalCodeFromOperatorCodeValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                string operatorCode = "VIN21211";
                uint? expectedResult = 1;

                ExecuteGetMissionInternalCodeFromOperatorCodeValid(accessor, operatorCode, expectedResult);
            }
        }

        /// <summary>Test GetMissionInternalCodeFromOperatorCode with no valid result expected.</summary>
        [Test]
        public void SivengRegiolisGetMissionInternalCodeFromOperatorCodeNotValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteGetMissionInternalCodeFromOperatorCodeNotValid(accessor);
            }
        }

        /// <summary>Test LanguageCodesExist with true statement expected.</summary>
        [Test]
        public void SivengRegiolisLanguageCodesExistsTrue()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                List<string> languagesIdList = new List<string>();
                languagesIdList.Add("spa");
                languagesIdList.Add("eng");
                languagesIdList.Add("fra");
                languagesIdList.Add("ger");
                languagesIdList.Add("ita");

                ExecuteLanguageCodesExistsTrue(accessor, languagesIdList);
            }
        }

        /// <summary>Test LanguageCodesExist with false statement expected.</summary>
        [Test]
        public void SivengRegiolisLanguageCodesExistsFalse()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteLanguageCodesExistsFalse(accessor);
            }
        }

        /// <summary>Test ServiceCodesExists with true statement expected.</summary>
        [Test]
        public void SivengRegiolisServiceCodesExistTrue()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                List<uint> onboardServiceCodeList = new List<uint>();
                onboardServiceCodeList.Add(0);
                onboardServiceCodeList.Add(4);
                onboardServiceCodeList.Add(9);

                ExecuteServiceCodesExistsTrue(accessor, onboardServiceCodeList);
            }
        }

        /// <summary>Test ServiceCodesExists with true statement expected.</summary>
        [Test]
        public void SivengRegiolisServiceCodesExistFalse()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteServiceCodesExistsFalse(accessor);
            }
        }

        /// <summary>Test RegionCodeExist with true statement expected.</summary>
        [Test]
        public void SivengRegiolisRegionCodeExistTrue()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteRegionCodeExistTrue(accessor);
            }
        }

        /// <summary>Test RegionCodeExist with false statement expected.</summary>
        [Test]
        public void SivengRegiolisRegionCodeExistFalse()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteRegionCodeExistFalse(accessor);
            }
        }

        /// <summary>Test GetStationOperatorCodeFromInternalCode with true statement expected.</summary>
        [Test]
        public void SivengRegiolisGetStationOperatorCodeFromInternalCodeValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                string stationOperatorCodeExpected = "0087212050AS";

                ExecuteGetStationOperatorCodeFromInternalCodeValid(accessor, stationOperatorCodeExpected);
            }
        }

        /// <summary>Test GetStationOperatorCodeFromInternalCode with false statement expected.</summary>
        [Test]
        public void SivengRegiolisGetStationOperatorCodeFromInternalCodeNotValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteGetStationOperatorCodeFromInternalCodeNotValid(accessor);
            }
        }

        /// <summary>Test GetStationInternalCodeFromOperatorCode with true statement expected.</summary>
        [Test]
        public void SivengRegiolisGetStationInternalCodeFromOperatorCodeValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                uint? stationInternalCodeExpected = 7;
                string stationOperatorCode = "0087212050AS";

                ExecuteGetStationInternalCodeFromOperatorCodeValid(accessor, stationOperatorCode, stationInternalCodeExpected);
            }
        }

        /// <summary>Test GetStationInternalCodeFromOperatorCode with false statement expected.</summary>
        [Test]
        public void SivengRegiolisGetStationInternalCodeFromOperatorCodeNotValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteGetStationInternalCodeFromOperatorCodeNotValid(accessor);
            }
        }

        /// <summary>Test RegionCodeExist with true statement expected.</summary>
        [Test]
        public void SivengRegiolisGetMissionRouteValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                uint missionId = 1;
                List<uint> missionRouteExpected = new List<uint>();
                missionRouteExpected.Add(2572);
                missionRouteExpected.Add(2574);
                missionRouteExpected.Add(4076);
                missionRouteExpected.Add(380);
                missionRouteExpected.Add(4238);
                missionRouteExpected.Add(653);
                missionRouteExpected.Add(2358);
                missionRouteExpected.Add(654);
                missionRouteExpected.Add(661);
                missionRouteExpected.Add(2283);
                missionRouteExpected.Add(2285);
                missionRouteExpected.Add(4138);

                ExecuteGetMissionRouteValid(accessor, missionId, missionRouteExpected);
            }
        }

        /// <summary>Test RegionCodeExist with false statement expected.</summary>
        [Test]
        public void SivengRegiolisGetMissionRouteNotValid()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                ExecuteGetMissionRouteNotValid(accessor);
            }
        }

        /// <summary>Test GetAllLanguages with results expected.</summary>
        [Test]
        public void SivengRegiolisGetAllLanguagesTests()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                List<string> languagesExpected = new List<string>();
                languagesExpected.Add("spa");
                languagesExpected.Add("eng");
                languagesExpected.Add("fra");
                languagesExpected.Add("ger");
                languagesExpected.Add("ita");

                ExecuteGetAllLanguagesTests(accessor, languagesExpected);
            }
        }

        /// <summary>Test GetStationList with results expected.</summary>
        [Test]
        public void SivengRegiolisGetStationListTest()
        {
            using (var accessor = new LmtDatabaseAccessor(_sivengRegiolisDBPath, CommonConfiguration.PlatformTypeEnum.SIVENG))
            {
                int numberOfStationExpected = 726;

                ExecuteGetStationListTest(accessor, numberOfStationExpected);
            }
        }

        #endregion

		#region LmtDatabaseAccessorTests - URBAN

		/// <summary>Test StationExists with true statement expected.</summary>
		[Test]
		public void UrbanStationExistsTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				string stationOperatorCode = "146";

				ExecuteStationExistsTrue(accessor, stationOperatorCode);
			}
		}

		/// <summary>Test StationExists with false statement expected.</summary>
		[Test]
		public void UrbanStationExistsFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				ExecuteStationExistsFalse(accessor);
			}
		}

		/// <summary>Test GetMissionInternalCodeFromOperatorCode with valid result expected.</summary>
		[Test]
		public void UrbanGetMissionInternalCodeFromOperatorCodeValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				string operatorCode = "231";
				uint? expectedResult = 3;

				ExecuteGetMissionInternalCodeFromOperatorCodeValid(accessor, operatorCode, expectedResult);
			}
		}

		/// <summary>Test GetMissionInternalCodeFromOperatorCode with no valid result expected.</summary>
		[Test]
		public void UrbanGetMissionInternalCodeFromOperatorCodeNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				ExecuteGetMissionInternalCodeFromOperatorCodeNotValid(accessor);
			}
		}

		/// <summary>Test LanguageCodesExist with true statement expected.</summary>
		[Test]
		public void UrbanLanguageCodesExistsTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				List<string> languagesIdList = new List<string>();
				languagesIdList.Add("en-US");
				languagesIdList.Add("fr-FR");

				ExecuteLanguageCodesExistsTrue(accessor, languagesIdList);
			}
		}

		/// <summary>Test LanguageCodesExist with false statement expected.</summary>
		[Test]
		public void UrbanLanguageCodesExistsFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				ExecuteLanguageCodesExistsFalse(accessor);
			}
		}

		/// <summary>Test ServiceCodesExists with true statement expected.</summary>
		[Test]
		public void UrbanServiceCodesExistTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				Assert.Throws<NotImplementedException>(() => ExecuteServiceCodesExistsTrue(accessor, null));
			}
		}

		/// <summary>Test ServiceCodesExists with true statement expected.</summary>
		[Test]
		public void UrbanServiceCodesExistFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				Assert.Throws<NotImplementedException>(() => ExecuteServiceCodesExistsFalse(accessor));
			}
		}

		/// <summary>Test RegionCodeExist with true statement expected.</summary>
		[Test]
		public void UrbanRegionCodeExistTrue()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				Assert.Throws<NotImplementedException>(() => ExecuteRegionCodeExistTrue(accessor));
			}
		}

		/// <summary>Test RegionCodeExist with false statement expected.</summary>
		[Test]
		public void UrbanRegionCodeExistFalse()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				Assert.Throws<NotImplementedException>(() => ExecuteRegionCodeExistFalse(accessor));
			}
		}

		/// <summary>Test GetStationOperatorCodeFromInternalCode with true statement expected.</summary>
		[Test]
		public void UrbanGetStationOperatorCodeFromInternalCodeValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				string stationOperatorCodeExpected = "234";

				ExecuteGetStationOperatorCodeFromInternalCodeValid(accessor, stationOperatorCodeExpected);
			}
		}

		/// <summary>Test GetStationOperatorCodeFromInternalCode with false statement expected.</summary>
		[Test]
		public void UrbanGetStationOperatorCodeFromInternalCodeNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				ExecuteGetStationOperatorCodeFromInternalCodeNotValid(accessor);
			}
		}

		/// <summary>Test GetStationInternalCodeFromOperatorCode with true statement expected.</summary>
		[Test]
		public void UrbanGetStationInternalCodeFromOperatorCodeValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				uint? stationInternalCodeExpected = 19;
				string stationOperatorCode = "262";

				ExecuteGetStationInternalCodeFromOperatorCodeValid(accessor, stationOperatorCode, stationInternalCodeExpected);
			}
		}

		/// <summary>Test GetStationInternalCodeFromOperatorCode with false statement expected.</summary>
		[Test]
		public void UrbanGetStationInternalCodeFromOperatorCodeNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				ExecuteGetStationInternalCodeFromOperatorCodeNotValid(accessor);
			}
		}

		/// <summary>Test RegionCodeExist with true statement expected.</summary>
		[Test]
		public void UrbanGetMissionRouteValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				uint missionId = 0;
				List<uint> missionRouteExpected = new List<uint>();
				missionRouteExpected.Add(2);
				missionRouteExpected.Add(3);
				missionRouteExpected.Add(4);
				missionRouteExpected.Add(5);
				missionRouteExpected.Add(6);
				missionRouteExpected.Add(7);
				missionRouteExpected.Add(8);
				missionRouteExpected.Add(9);
				missionRouteExpected.Add(10);
				missionRouteExpected.Add(11);
				missionRouteExpected.Add(12);
				missionRouteExpected.Add(13);
				missionRouteExpected.Add(14);
				missionRouteExpected.Add(15);
				missionRouteExpected.Add(16);
				missionRouteExpected.Add(17);
				missionRouteExpected.Add(18);
				missionRouteExpected.Add(0);
				missionRouteExpected.Add(19);
				missionRouteExpected.Add(20);
				missionRouteExpected.Add(21);
				missionRouteExpected.Add(22);
				missionRouteExpected.Add(23);
				missionRouteExpected.Add(24);
				missionRouteExpected.Add(25);
				missionRouteExpected.Add(26);
				missionRouteExpected.Add(27);
				missionRouteExpected.Add(28);
				missionRouteExpected.Add(29);
				missionRouteExpected.Add(30);
				missionRouteExpected.Add(1);

				ExecuteGetMissionRouteValid(accessor, missionId, missionRouteExpected);
			}
		}

		/// <summary>Test RegionCodeExist with false statement expected.</summary>
		[Test]
		public void UrbanGetMissionRouteNotValid()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				ExecuteGetMissionRouteNotValid(accessor);
			}
		}

		/// <summary>Test GetAllLanguages with results expected.</summary>
		[Test]
		public void UrbanGetAllLanguagesTests()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				List<string> languagesExpected = new List<string>();
				languagesExpected.Add("en-US");
				languagesExpected.Add("fr-FR");

				ExecuteGetAllLanguagesTests(accessor, languagesExpected);
			}
		}

		/// <summary>Test GetStationList with results expected.</summary>
		[Test]
		public void UrbanGetStationListTest()
		{
			using (var accessor = new LmtDatabaseAccessor(_urbanDBPath, CommonConfiguration.PlatformTypeEnum.URBAN))
			{
				int numberOfStationExpected = 31;

				ExecuteGetStationListTest(accessor, numberOfStationExpected);
			}
		}

		#endregion

		#region execute

		/// <summary>Executes the station exists true operation.</summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="stationOperatorCode">The operator code.</param>
		private static void ExecuteStationExistsTrue(LmtDatabaseAccessor accessor, string stationOperatorCode)
		{
			bool result = false;

			result = accessor.StationExists(stationOperatorCode);

			Assert.IsTrue(result);
		}

		/// <summary>Executes the station exists false operation.</summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteStationExistsFalse(LmtDatabaseAccessor accessor)
		{
			string stationOperatorCode = "NotValid";

			bool result = true;

			result = accessor.StationExists(stationOperatorCode);

			Assert.IsFalse(result);
		}

		/// <summary>
		/// Executes the get mission internal code from operator code valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="operatorCode">The operator code.</param>
		/// <param name="expectedResult">The expected internal code result.</param>
		private static void ExecuteGetMissionInternalCodeFromOperatorCodeValid(LmtDatabaseAccessor accessor, string operatorCode, uint? expectedResult)
		{
			uint? result = null;

			result = accessor.GetMissionInternalCodeFromOperatorCode(operatorCode);

			Assert.IsNotNull(result);
			Assert.AreEqual(expectedResult, result);
		}

		/// <summary>
		/// Executes the get mission internal code from operator code not valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteGetMissionInternalCodeFromOperatorCodeNotValid(LmtDatabaseAccessor accessor)
		{
			string operatorCode = "NotValid";

			uint? result = 1;

			result = accessor.GetMissionInternalCodeFromOperatorCode(operatorCode);

			Assert.IsNull(result);
		}

		/// <summary>Executes the language codes exists true operation.</summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="languagesIdList">The list of language to check in database.</param>
		private static void ExecuteLanguageCodesExistsTrue(LmtDatabaseAccessor accessor, List<string> languagesIdList)
		{
			bool result = false;

			result = accessor.LanguageCodesExist(languagesIdList);

			Assert.IsTrue(result);
		}

		/// <summary>Executes the language codes exists false operation.</summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteLanguageCodesExistsFalse(LmtDatabaseAccessor accessor)
		{
			List<string> languagesIdList = new List<string>();
			languagesIdList.Add("na1");
			languagesIdList.Add("na2");
			languagesIdList.Add("na3");
			bool result = true;

			result = accessor.LanguageCodesExist(languagesIdList);

			Assert.IsFalse(result);
		}

		/// <summary>Executes the ServiceCodeExistsTrue operation.</summary>
		/// <param name="accessor">The accessor.</param>
        private static void ExecuteServiceCodesExistsTrue(LmtDatabaseAccessor accessor, List<uint> onboardServiceCodeList)
		{
			bool result = false;

			result = accessor.ServiceCodesExist(onboardServiceCodeList);

			Assert.IsTrue(result);
		}

		/// <summary>Executes the ServiceCodeExistsFalse operation.</summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteServiceCodesExistsFalse(LmtDatabaseAccessor accessor)
		{
			List<uint> onboardServiceCodeList = new List<uint>();
			onboardServiceCodeList.Add(35987);
			onboardServiceCodeList.Add(55942);
			onboardServiceCodeList.Add(15571);
			onboardServiceCodeList.Add(32156);

			bool result = true;

			result = accessor.ServiceCodesExist(onboardServiceCodeList);

			Assert.IsFalse(result);
		}

		/// <summary>Executes the region code exist true operation.</summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteRegionCodeExistTrue(LmtDatabaseAccessor accessor)
		{
			uint regionCode = 1;

			bool result = false;

			result = accessor.RegionCodeExist(regionCode);

			Assert.IsTrue(result);
		}

		/// <summary>Executes the region code exist false operation.</summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteRegionCodeExistFalse(LmtDatabaseAccessor accessor)
		{
			uint regionCode = 358446;

			bool result = true;

			result = accessor.RegionCodeExist(regionCode);

			Assert.IsFalse(result);
		}

		/// <summary>
		/// Executes the get station operator code from internal code valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="stationOperatorCodeExpected">The operator code expected.</param>
		private static void ExecuteGetStationOperatorCodeFromInternalCodeValid(LmtDatabaseAccessor accessor, string stationOperatorCodeExpected)
		{
			uint stationInternalCode = 7;
			string stationOperatorCodeResult = string.Empty;

			stationOperatorCodeResult = accessor.GetStationOperatorCodeFromInternalCode(stationInternalCode);

			Assert.AreEqual(stationOperatorCodeExpected, stationOperatorCodeResult);
		}

		/// <summary>
		/// Executes the get station operator code from internal code valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteGetStationOperatorCodeFromInternalCodeNotValid(LmtDatabaseAccessor accessor)
		{
			uint stationInternalCode = 548681;
			string stationOperatorCodeExpected = string.Empty;
			string stationOperatorCodeResult = "NotValid";

			stationOperatorCodeResult = accessor.GetStationOperatorCodeFromInternalCode(stationInternalCode);

			Assert.AreEqual(stationOperatorCodeExpected, stationOperatorCodeResult);
		}

		/// <summary>
		/// Executes the get station internal code from operator code valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="stationOperatorCode">The station operator code.</param>
		/// <param name="stationInternalCodeExpected">The internal code expected.</param>
		private static void ExecuteGetStationInternalCodeFromOperatorCodeValid(LmtDatabaseAccessor accessor, string stationOperatorCode, uint? stationInternalCodeExpected)
		{
			uint? stationInternalCodeResult = null;

			stationInternalCodeResult = accessor.GetStationInternalCodeFromOperatorCode(stationOperatorCode);

			Assert.IsNotNull(stationInternalCodeResult);
			Assert.AreEqual(stationInternalCodeExpected, stationInternalCodeResult);
		}

		/// <summary>
		/// Executes the get station internal code from operator code valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteGetStationInternalCodeFromOperatorCodeNotValid(LmtDatabaseAccessor accessor)
		{
			uint? stationInternalCodeResult = 5555;
			string stationOperatorCode = "NotValid";

			stationInternalCodeResult = accessor.GetStationInternalCodeFromOperatorCode(stationOperatorCode);

			Assert.IsNull(stationInternalCodeResult);
		}

		/// <summary>
		/// Executes the get mission route valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="missionId">The mission identifier.</param>
		/// <param name="missionRouteExpected">The expected result.</param>
		private static void ExecuteGetMissionRouteValid(LmtDatabaseAccessor accessor, uint missionId, List<uint> missionRouteExpected)
		{
			List<uint> missionRouteResult = new List<uint>();

			missionRouteResult = accessor.GetMissionRoute(missionId);

			Assert.AreEqual(missionRouteExpected, missionRouteResult);
		}

		/// <summary>
		/// Executes the get mission route not valid operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		private static void ExecuteGetMissionRouteNotValid(LmtDatabaseAccessor accessor)
		{
			uint missionId = 999999;
			List<uint> missionRouteResult = new List<uint>();
			List<uint> missionRouteExpected = new List<uint>();

			missionRouteResult = accessor.GetMissionRoute(missionId);

			Assert.AreEqual(missionRouteExpected, missionRouteResult);
		}

		/// <summary>
		/// Executes the get all languages operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="languagesExpected">The list of expected languages.</param>
		private static void ExecuteGetAllLanguagesTests(LmtDatabaseAccessor accessor, List<string> languagesExpected)
		{
			List<string> languagesResult = new List<string>();

			languagesResult = accessor.GetAllLanguages();

			Assert.AreEqual(languagesResult, languagesExpected);
		}

		/// <summary>
		/// Executes the get station list operation.
		/// </summary>
		/// <param name="accessor">The accessor.</param>
		/// <param name="numberOfStationExpected">The number of station expected from database.</param>
		private static void ExecuteGetStationListTest(LmtDatabaseAccessor accessor, int numberOfStationExpected)
		{
			List<PIS.Ground.Core.PackageAccess.Station> stationListResult = new List<PIS.Ground.Core.PackageAccess.Station>();

			stationListResult = accessor.GetStationList();

			Assert.AreEqual(numberOfStationExpected, stationListResult.Count);
		}

		#endregion
	}
}