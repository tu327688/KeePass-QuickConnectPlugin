using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KeePassLib;
using KeePassLib.Security;

namespace QuickConnectPlugin {

    public class HostPwEntry : IHostPwEntry {

        private String ipAddress;
        public String IPAddress {
            get {
                if (this.ipAddress == null) {
                    this.ipAddress = this.pwEntry.Strings.ReadSafe(this.fieldsMapper.HostAddress);
                }
                return ipAddress;
            }
        }

        private IList<ConnectionMethodType> connectionMethods = null;
        public ICollection<ConnectionMethodType> ConnectionMethods {
            get {
                if (this.connectionMethods == null) {

                    if (this.fieldsMapper.ConnectionMethod == null) {
                        return new Collection<ConnectionMethodType>();
                    }

                    var value = this.pwEntry.Strings.ReadSafe(this.fieldsMapper.ConnectionMethod);
                    this.connectionMethods = new List<ConnectionMethodType>(ConnectionMethodTypeUtils.GetConnectionMethodsFromString(value));
                }
                return new Collection<ConnectionMethodType>(this.connectionMethods);
            }
        }

        private string additionalOptions = null;
        public string AdditionalOptions {
            get {
                if (this.additionalOptions == null) {

                    if (this.fieldsMapper.AdditionalOptions == null) {
                        return null;
                    }

                    // Read the compiled string to enable the use of placeholders in this field, e.g.: {PASSWORD}
                    this.additionalOptions = PwEntryUtils.ReadCompiledSafeString(this.pwDatabase, this.pwEntry, this.fieldsMapper.AdditionalOptions);
                }
                return this.additionalOptions;
            }
        }

        private PwEntry pwEntry;
        private PwDatabase pwDatabase;
        private IFieldMapper fieldsMapper;

        public HostPwEntry(PwEntry pwEntry, PwDatabase pwDatabase, IFieldMapper fieldMapper) {
            this.pwEntry = pwEntry;
            this.pwDatabase = pwDatabase;
            this.fieldsMapper = fieldMapper;
        }

        public bool HasIPAddress {
            get {
                return !String.IsNullOrEmpty(this.IPAddress);
            }
        }

        public bool HasConnectionMethods {
            get {
                return this.ConnectionMethods.Count > 0;
            }
        }

        public String Title {
            get {
                return this.pwEntry.Strings.Get(PwDefs.TitleField).ReadString();
            }
        }

        public String GetUsername() {
            return PwEntryUtils.ReadCompiledSafeString(this.pwDatabase, this.pwEntry, PwDefs.UserNameField);
        }

        public String GetPassword() {
            return PwEntryUtils.ReadCompiledSafeString(this.pwDatabase, this.pwEntry, PwDefs.PasswordField);
        }

        public DateTime LastModificationTime {
            get  {
                return this.pwEntry.LastModificationTime;
            }
            set {
                this.pwEntry.LastModificationTime = value;
            }
        }

        public void UpdatePassword(string newPassword) {
            this.pwDatabase.RootGroup.FindEntry(this.pwEntry.Uuid, true).Strings.Set(PwDefs.PasswordField, new ProtectedString(true, newPassword));
        }
    }
}
