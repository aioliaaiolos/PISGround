﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3655
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PIS.Ground.RealTime {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Logs {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Logs() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RealTime.Logs", typeof(Logs).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail do get station list for element {0}..
        /// </summary>
        internal static string ERROR_ACCESSING_STATIONLIST_FOR_ELEMENT {
            get {
                return ResourceManager.GetString("ERROR_ACCESSING_STATIONLIST_FOR_ELEMENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fail to send request to embedded {0}..
        /// </summary>
        internal static string ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED {
            get {
                return ResourceManager.GetString("ERROR_FAILED_SEND_REQUEST_TO_EMBEDDED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while requesting service {0} data for element {1} : {2}..
        /// </summary>
        internal static string ERROR_GET_SERVICE_DATA_FOR_ELEMENT {
            get {
                return ResourceManager.GetString("ERROR_GET_SERVICE_DATA_FOR_ELEMENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to open package LMT version {0} for element {1}..
        /// </summary>
        internal static string ERROR_RETRIEVESTATIONLIST_CANT_OPEN_PACKAGE {
            get {
                return ResourceManager.GetString("ERROR_RETRIEVESTATIONLIST_CANT_OPEN_PACKAGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to find LMT database inside package version {0} for element {1}..
        /// </summary>
        internal static string ERROR_RETRIEVESTATIONLIST_LMT_DB_NOT_FOUND {
            get {
                return ResourceManager.GetString("ERROR_RETRIEVESTATIONLIST_LMT_DB_NOT_FOUND", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Embedded LMT version {0} not found in RemoteDataStore..
        /// </summary>
        internal static string ERROR_RETRIEVESTATIONLIST_UNKNOWN_EMBEDDED_LMT {
            get {
                return ResourceManager.GetString("ERROR_RETRIEVESTATIONLIST_UNKNOWN_EMBEDDED_LMT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurs when using RemoteDataStoreProxy..
        /// </summary>
        internal static string ERROR_UNKNOWN {
            get {
                return ResourceManager.GetString("ERROR_UNKNOWN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No elements are currently running mission {0}..
        /// </summary>
        internal static string INFO_NO_ELEMENTS_FOR_MISSION {
            get {
                return ResourceManager.GetString("INFO_NO_ELEMENTS_FOR_MISSION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Response from element {0} for {1} data processing for mission {2}, station {3}:\n.
        /// </summary>
        internal static string RESULT_PROCESS_CMD {
            get {
                return ResourceManager.GetString("RESULT_PROCESS_CMD", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to For parameter {0}, result message is {1} ({2}).
        /// </summary>
        internal static string RESULT_PROCESS_PARAMETER {
            get {
                return ResourceManager.GetString("RESULT_PROCESS_PARAMETER", resourceCulture);
            }
        }
    }
}
