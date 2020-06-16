using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXProject : PBXObject
	{
		protected string MAINGROUP_KEY = "mainGroup";
		protected string KNOWN_REGIONS_KEY = "knownRegions";
        protected PBXDictionary _capabilities;

        protected bool _clearedLoc = false;

		public PBXProject() : base() {
		}
		
		public PBXProject( string guid, PBXDictionary dictionary ) : base( guid, dictionary ) {
		}
		
		public string mainGroupID {
			get {
				return (string)_data[ MAINGROUP_KEY ];
			}
		}

		public PBXList knownRegions {
			get {
				return (PBXList)_data[ KNOWN_REGIONS_KEY ];
			}
		}

		public void AddRegion(string region) {
			if (!_clearedLoc)
			{
				// Only include localizations we explicitly specify
				knownRegions.Clear();
				_clearedLoc = true;
			}

			knownRegions.Add(region);
		}

        public bool SetSystemCapabilities(string key, string value)
        {
            if (_capabilities == null)
            {
                PBXList targetList = (PBXList)_data["targets"];
                string targetKey = (string)targetList[0];
                PBXDictionary attribute = (PBXDictionary)_data["attributes"];
                PBXDictionary targetAttribute = (PBXDictionary)attribute["TargetAttributes"];
                PBXDictionary mainTarget;
                if (!targetAttribute.ContainsKey(targetKey))
                {
                    mainTarget = new PBXDictionary();
                    targetAttribute.Add(targetKey, mainTarget);
                    _capabilities = new PBXDictionary();
                    mainTarget.Add("SystemCapabilities", _capabilities);
                }
                else
                {
                    mainTarget = (PBXDictionary)targetAttribute[targetKey];
                    _capabilities = (PBXDictionary)mainTarget["SystemCapabilities"];
                }

            }

            if (_capabilities.ContainsKey(key))
            {
                ((PBXDictionary)_capabilities[key])["enabled"] = value;
            }
            else
            {
                PBXDictionary valueDic = new PBXDictionary();
                valueDic.Add("enabled", value);
                _capabilities.Add(key, valueDic);
            }

            return true;
        }
    }
}
