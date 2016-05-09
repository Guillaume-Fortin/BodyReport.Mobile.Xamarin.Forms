using BodyReportMobile.Core.Crud.Module;
using BodyReportMobile.Core.Crud.Transformer;
using BodyReportMobile.Core.Framework;
using Message;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.ServiceManagers
{
    public class MuscularGroupManager : ServiceManager
    {
        MuscularGroupModule _module;
        public MuscularGroupManager(SQLiteConnection dbContext) : base(dbContext)
		{
            _module = new MuscularGroupModule(_dbContext);
        }

        public List<MuscularGroup> FindMuscularGroups()
        {
            var muscularGroupList = _module.Find();
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

        internal List<MuscularGroup> UpdateMuscularGroupList(List<MuscularGroup> muscularGroupList)
        {
            List<MuscularGroup> list = new List<MuscularGroup>();
            foreach (var muscularGroup in muscularGroupList)
            {
                //Translation.UpdateInDB(MuscleTransformer.GetTranslationKey(muscle.Id), muscle.Name, _dbContext);
                list.Add(_module.Update(muscularGroup));
            }
            return list;
        }
    }
}
