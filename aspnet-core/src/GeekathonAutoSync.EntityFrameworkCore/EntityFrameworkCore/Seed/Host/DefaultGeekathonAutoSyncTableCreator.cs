using Abp.Authorization.Users;
using Abp.Localization;
using Abp.MultiTenancy;
using GeekathonAutoSync.Authorization.Users;
using GeekathonAutoSync.BackUpFrequencys;
using GeekathonAutoSync.BackUPTypes;
using GeekathonAutoSync.CloudStorages;
using GeekathonAutoSync.DBTypes;
using GeekathonAutoSync.StorageMasterTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekathonAutoSync.EntityFrameworkCore.Seed.Host
{
    public class DefaultGeekathonAutoSyncTableCreator
    {
        public static List<StorageMasterType> InitialStorageMasterTypes => GetInitialStorageMasterTypes();
        public static List<BackUPType> InitialBackUPTypes => GetInitialBackUPTypes();
        public static List<DBType> InitialDBTypes => GetInitialDBTypes();
        public static List<BackUpFrequency> InitialBackUpFrequencies => GetInitialBackUpFrequencies();
        public static List<CloudStorage> InitialCloudStorages => GetInitialCloudStorages();
        
        private readonly GeekathonAutoSyncDbContext _context;
        public DefaultGeekathonAutoSyncTableCreator(GeekathonAutoSyncDbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            CreateStorageMasterTypes();
            CreateBackUPTypes();
            CreateDBTypes();
            CreateBackUpFrequencies();
            CreateCloudStorages();
        }

        #region StorageMasterType
        private void CreateStorageMasterTypes()
        {
            foreach (var storageType in InitialStorageMasterTypes)
            {
                AddStorageMasterTypeIfNotExists(storageType);
            }
        }
        private void AddStorageMasterTypeIfNotExists(StorageMasterType storageType)
        {
            if (_context.StorageMasterTypes.IgnoreQueryFilters().Any(l => l.Name == storageType.Name))
            {
                return;
            }

            _context.StorageMasterTypes.Add(storageType);
            _context.SaveChanges();
        }
        private static List<StorageMasterType> GetInitialStorageMasterTypes()
        {
            return new List<StorageMasterType>
            {
                new StorageMasterType(Guid.NewGuid(), "Public Cloud"),
                new StorageMasterType(Guid.NewGuid(), "GeekSync Infrastructure Cluste"),
                new StorageMasterType(Guid.NewGuid(), "Network File System")
            };
        }
        #endregion

        #region BackUPType
        private void CreateBackUPTypes()
        {
            foreach (var backupType in InitialBackUPTypes)
            {
                AddBackUPTypeIfNotExists(backupType);
            }
        }
        private void AddBackUPTypeIfNotExists(BackUPType backupType)
        {
            if (_context.BackUPTypes.IgnoreQueryFilters().Any(l => l.Name == backupType.Name))
            {
                return;
            }

            _context.BackUPTypes.Add(backupType);
            _context.SaveChanges();
        }
        private static List<BackUPType> GetInitialBackUPTypes()
        {
            return new List<BackUPType>
            {
                new BackUPType(Guid.NewGuid(), "DataBase"),
                new BackUPType(Guid.NewGuid(), "Application Files"),
                new BackUPType(Guid.NewGuid(), "Specific File")
            };
        }
        #endregion

        #region DBType		
        private void CreateDBTypes()
        {
            foreach (var dBType in InitialDBTypes)
            {
                AddDBTypeIfNotExists(dBType);
            }
        }
        private void AddDBTypeIfNotExists(DBType dBType)
        {
            if (_context.DBTypes.IgnoreQueryFilters().Any(l => l.Name == dBType.Name))
            {
                return;
            }

            _context.DBTypes.Add(dBType);
            _context.SaveChanges();
        }
        private static List<DBType> GetInitialDBTypes()
        {
            return new List<DBType>
            {
                new DBType(Guid.NewGuid(), "PostgreSQL"),
                new DBType(Guid.NewGuid(), "Microsoft SQL Server"),
                new DBType(Guid.NewGuid(), "Oracle Database"),
                new DBType(Guid.NewGuid(), "MySQL"),
                new DBType(Guid.NewGuid(), "MongoDB")
            };
        }
        #endregion

        #region BackUpFrequency
        private void CreateBackUpFrequencies()
        {
            foreach (var backUpFrequency in InitialBackUpFrequencies)
            {
                AddBackUpFrequencyIfNotExists(backUpFrequency);
            }
        }
        private void AddBackUpFrequencyIfNotExists(BackUpFrequency backUpFrequency)
        {
            if (_context.BackupFrequencies.IgnoreQueryFilters().Any(l => l.Name == backUpFrequency.Name))
            {
                return;
            }

            _context.BackupFrequencies.Add(backUpFrequency);
            _context.SaveChanges();
        }
        private static List<BackUpFrequency> GetInitialBackUpFrequencies()
        {
            return new List<BackUpFrequency>
            {
                new BackUpFrequency(Guid.NewGuid(), "Hourly"),
                new BackUpFrequency(Guid.NewGuid(), "Daily"),
                new BackUpFrequency(Guid.NewGuid(), "Weekly"),
                new BackUpFrequency(Guid.NewGuid(), "Monthly"),
                new BackUpFrequency(Guid.NewGuid(), "Yearly")
            };
        }
        #endregion

        #region CloudStorage
        private void CreateCloudStorages()
        {
            foreach (var cloudStorage in InitialCloudStorages)
            {
                AddCloudStorageIfNotExists(cloudStorage);
            }
        }
        private void AddCloudStorageIfNotExists(CloudStorage cloudStorage)
        {
            if (_context.CloudStorages.IgnoreQueryFilters().Any(l => l.Name == cloudStorage.Name))
            {
                return;
            }

            _context.CloudStorages.Add(cloudStorage);
            _context.SaveChanges();
        }
        private static List<CloudStorage> GetInitialCloudStorages()
        {
            return new List<CloudStorage>
            {
                new CloudStorage(Guid.NewGuid(), "Amazon S3"),
                new CloudStorage(Guid.NewGuid(), "Microsoft Azure"),
                new CloudStorage(Guid.NewGuid(), "Google Cloud"),
                new CloudStorage(Guid.NewGuid(), "Alibaba Cloud")
            };
        }
        #endregion
    }
}
