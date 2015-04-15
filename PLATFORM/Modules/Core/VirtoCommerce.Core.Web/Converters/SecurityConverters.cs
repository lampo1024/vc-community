﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Omu.ValueInjecter;
using VirtoCommerce.Foundation.Frameworks;
using VirtoCommerce.Foundation.Frameworks.ConventionInjections;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using foundation = VirtoCommerce.Foundation.Security.Model;
using coreModel = VirtoCommerce.Framework.Web.Security;

namespace VirtoCommerce.CoreModule.Web.Converters
{
    public static class SecurityConverters
    {
        public static foundation.Role ToFoundation(this coreModel.RoleDescriptor source)
        {
            var result = new foundation.Role
            {
                Name = source.Name
            };

            if (source.Id != null)
                result.RoleId = source.Id;

            result.RolePermissions = new NullCollection<foundation.RolePermission>();

            if (source.Permissions != null)
            {
                result.RolePermissions = new ObservableCollection<foundation.RolePermission>(source.Permissions.Select(p => new foundation.RolePermission { PermissionId = p.Id }));
            }

            return result;
        }

        public static foundation.Permission ToFoundation(this coreModel.PermissionDescriptor source)
        {
            var result = new foundation.Permission
            {
                PermissionId = source.Id,
                Name = source.Name
            };

            return result;
        }

        public static coreModel.RoleDescriptor ToCoreModel(this foundation.Role source, bool fillPermissions)
        {
            var result = new coreModel.RoleDescriptor
            {
                Id = source.RoleId,
                Name = source.Name,
            };

            if (fillPermissions && source.RolePermissions != null)
            {
                result.Permissions = source.RolePermissions.Select(rp => rp.Permission.ToCoreModel()).ToArray();
            }

            return result;
        }

        public static coreModel.PermissionDescriptor ToCoreModel(this foundation.Permission source)
        {
            var result = new coreModel.PermissionDescriptor
            {
                Id = source.PermissionId,
                Name = source.Name,
            };

            return result;
        }

        public static void Patch(this foundation.Role source, foundation.Role target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var patchInjection = new PatchInjection<foundation.Role>(x => x.Name);
            target.InjectFrom(patchInjection, source);

            if (!source.RolePermissions.IsNullCollection())
            {
                var settingComparer = AnonymousComparer.Create((foundation.RolePermission rp) => rp.RolePermissionId);
                source.RolePermissions.Patch(target.RolePermissions, settingComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }
        }

        public static void Patch(this foundation.RolePermission source, foundation.RolePermission target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var patchInjection = new PatchInjection<foundation.RolePermission>(x => x.RoleId, x => x.PermissionId);
            target.InjectFrom(patchInjection, source);
        }

        public static void Patch(this foundation.Permission source, foundation.Permission target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var patchInjection = new PatchInjection<foundation.Permission>(x => x.Name);
            target.InjectFrom(patchInjection, source);
        }
    }
}
