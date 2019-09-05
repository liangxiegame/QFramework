using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using QF.GraphDesigner;
using Invert.Data;
using QF;
using JetBrains.Annotations;
using QF.GraphDesigner.Unity.KoinoniaSystem.Classes;
using QF.GraphDesigner.Unity.KoinoniaSystem.Data;
using UnityEngine;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Services
{
    public class DescriptorsService : IDesctiptorsService
    {
        private KoinoniaSystem _koinoniaSystem;

        public KoinoniaSystem KoinoniaSystem
        {
            get { return _koinoniaSystem ?? (_koinoniaSystem = InvertApplication.Container.Resolve<KoinoniaSystem>()); }
            set { _koinoniaSystem = value; }
        }

        public IRepository Cache { get;set; }

        public DescriptorsService(IRepository cache)
        {
            Cache = cache;
        }

        private string RequestPackageDescriptorById(string id)
        {
            //TODO KOINONIA: make request to web... should block the thread?
            Thread.Sleep(300);
            var text = Resources.Load<TextAsset>("Package");
            var packs = InvertJsonExtensions.DeserializeObject<List<UFramePackageDescriptor>>(text.text);
            var pack = packs.FirstOrDefault(p => p.Id == id);
            return InvertJsonExtensions.SerializeObject(pack);
        }

        private string RequestPackageRevisionDescriptorById(string id)
        {
            //TODO KOINONIA: make request to web... should block the thread?
            Thread.Sleep(300);
            var text = Resources.Load<TextAsset>("Revisions");
            var packs = InvertJsonExtensions.DeserializeObject<List<UFramePackageRevisionDescriptor>>(text.text);
            var pack = packs.FirstOrDefault(p => p.Id == id);
            return InvertJsonExtensions.SerializeObject(pack);
        }

        private IEnumerable<UFramePackageRevisionDescriptor> RequestPackageRevisionsByIds(IEnumerable<string> revisionsToRequest)
        {
            Thread.Sleep(300);
            var text = Resources.Load<TextAsset>("Revisions");
            var packs = InvertJsonExtensions.DeserializeObject<List<UFramePackageRevisionDescriptor>>(text.text).Where(r=>revisionsToRequest.Contains(r.Id));
            return packs;
        }

        private List<UFramePackageDescriptor> RequestLatestPackages()
        {
            //TODO KOINONIA: make request to web... should block the thread?
            Thread.Sleep(300);
            var text = Resources.Load<TextAsset>("Package");
            var packs = InvertJsonExtensions.DeserializeObject<List<UFramePackageDescriptor>>(text.text);
            return packs;
        }

        public string RequestPackageRevisionByPackageIdAndTag(string packageId, string tag)
        {
            //TODO KOINONIA: make request to web... should block the thread?
            Thread.Sleep(300);
            var text = Resources.Load<TextAsset>("Revisions");
            var packs = InvertJsonExtensions.DeserializeObject<List<UFramePackageRevisionDescriptor>>(text.text);
            var pack = packs.FirstOrDefault(p => p.VersionTag == tag && p.PackageId == packageId);
            return InvertJsonExtensions.SerializeObject(pack);
        }

        public UFramePackageDescriptor GetPackageDescriptorById(string id)
        {

            if (KoinoniaSystem.IsRemoteServerAvailable && CachedPackageDescriptorNeedsUpdate(id))
            {
                var json = RequestPackageDescriptorById(id);
                var descriptor = InvertJsonExtensions.DeserializeObject<UFramePackageDescriptor>(json);
                UpdateCacheWithPackageDescriptor(descriptor);
                return descriptor;
            }
            
            return Cache.All<UFramePackageDescriptor>().FirstOrDefault(d=>d.Id == id);
        }

        public UFramePackageDescriptor GetPackageDescriptorByRevision(UFramePackageRevisionDescriptor revision)
        {
            return GetPackageDescriptorById(revision.Id);
        }

        public UFramePackageDescriptor GetPackageDescriptorByPackage(UFramePackage package)
        {
            return GetPackageDescriptorById(package.Id);
        }

        public UFramePackageRevisionDescriptor GetRevisionById(string id)
        {
            if (KoinoniaSystem.IsRemoteServerAvailable && CachedPackageDescriptorNeedsUpdate(id))
            {
                var json = RequestPackageRevisionDescriptorById(id);
                var descriptor = InvertJsonExtensions.DeserializeObject<UFramePackageRevisionDescriptor>(json);
                UpdateCacheWithPackageRevisionDescriptor(descriptor);
                return descriptor;
            }

            return Cache.All<UFramePackageRevisionDescriptor>().FirstOrDefault(d => d.Id == id);
        }

        public UFramePackageRevisionDescriptor GetRevisionDescriptorByPackageIdAndTag(string packageId, string tag)
        {
            var possibleRevision =
                Cache.All<UFramePackageRevisionDescriptor>().FirstOrDefault(r => r.PackageId == packageId && r.VersionTag == tag);

            if (possibleRevision == null || CachedRevisionDescriptorNeedsUpdate(possibleRevision.Id))
            {
                var json = RequestPackageRevisionByPackageIdAndTag(packageId,tag);
                var descriptor = InvertJsonExtensions.DeserializeObject<UFramePackageRevisionDescriptor>(json);
                UpdateCacheWithPackageRevisionDescriptor(descriptor);
                return descriptor;
            }
            
            return possibleRevision;
        }

        public IEnumerable<UFramePackageRevisionDescriptor> GetRevisionsByProject(UFramePackageDescriptor package)
        {
            var revisionsToRequest = package.RevisionIds.Where(CachedRevisionDescriptorNeedsUpdate).ToArray();
            var upToDateRevisions = package.RevisionIds.Except(revisionsToRequest);
            var newRevisions = RequestPackageRevisionsByIds(revisionsToRequest).ToArray();

            foreach (var revision in newRevisions)
            {
                UpdateCacheWithPackageRevisionDescriptor(revision);
            }

            Cache.Commit();
            //Get new updated revisions
            //Concat with up-to-date shit
            //?????
            //Profit
            return upToDateRevisions.Select(_=>GetRevisionById(_)).Concat(newRevisions);

        }

        public IEnumerable<UFramePackageDescriptor> GetLatest()
        {
            if(!KoinoniaSystem.IsRemoteServerAvailable) yield break;

            var latest = RequestLatestPackages();

            foreach (var uFramePackageDescriptor in RequestLatestPackages())
            {
                UpdateCacheWithPackageDescriptor(uFramePackageDescriptor);
                yield return uFramePackageDescriptor;
            }

            Cache.Commit();

        }

        private void UpdateCacheWithPackageDescriptor(UFramePackageDescriptor descriptor)
        {
            var oldEntry = Cache.All<UFramePackageDescriptor>().FirstOrDefault(d=>d.Id == descriptor.Id);
            if (oldEntry != null) Cache.Remove(oldEntry);
            descriptor.CacheExpireTime = DateTime.Now.AddMinutes(30);
            Cache.Add(descriptor);
            Cache.Commit();
        }      
        
        private void UpdateCacheWithPackageRevisionDescriptor(UFramePackageRevisionDescriptor descriptor)
        {
            var oldEntry = Cache.All<UFramePackageRevisionDescriptor>().FirstOrDefault(d=>d.Id == descriptor.Id);
            if (oldEntry != null) Cache.Remove(oldEntry);
            descriptor.CacheExpireTime = DateTime.Now.AddMinutes(30);
            Cache.Add(descriptor);
            Cache.Commit();
        } 

        private bool CachedPackageDescriptorNeedsUpdate(string id)
        {
            var oldEntry = Cache.All<UFramePackageDescriptor>().FirstOrDefault(d => d.Id == id);
            return oldEntry == null || oldEntry.CacheExpireTime < DateTime.Now;
        }

        private bool CachedRevisionDescriptorNeedsUpdate(string id)
        {
            var oldEntry = Cache.All<UFramePackageRevisionDescriptor>().FirstOrDefault(d => d.Id == id);
            return oldEntry == null || oldEntry.CacheExpireTime < DateTime.Now;
        }

        public IEnumerable<UFramePackageDescriptor> Search()
        {
            throw new NotImplementedException();
        }
    }
}
