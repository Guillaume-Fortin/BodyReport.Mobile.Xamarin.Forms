using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using BodyReportMobile.Core.Data;

namespace BodyReportMobile.Core.Manager
{
    public class MuscularGroupManager : BodyReportManager
    {
        MuscularGroupModule _module;
        public MuscularGroupManager(ApplicationDbContext dbContext) : base(dbContext)
		{
            _module = new MuscularGroupModule(DbContext);
        }

        public List<MuscularGroup> FindMuscularGroups(MuscularGroupCriteria muscularGroupCriteria = null)
        {
            var muscularGroupList = _module.Find(muscularGroupCriteria);
            if (muscularGroupList != null)
            {
                foreach (var muscularGroup in muscularGroupList)
                {
                    if (muscularGroup != null)
                        muscularGroup.Name = Translation.GetInDB(MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id));
                }
            }

            return muscularGroupList;
        }

        internal MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            var muscularGroup =_module.Get(key);
            if(muscularGroup != null)
            {
                muscularGroup.Name = Translation.GetInDB(MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id));
            }
            return muscularGroup;
        }

        internal void DeleteMuscularGroup(MuscularGroupKey key)
        {
            //Update Translation Name
            //Translation.DeleteInDB(MuscularGroupTransformer.GetTranslationKey(key.Id), _dbContext);
            _module.Delete(key);
        }

        internal MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            //Update Translation Name
            //Translation.UpdateInDB(MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id), muscularGroup.Name, _dbContext);

            return _module.Update(muscularGroup);
        }
    }
}
