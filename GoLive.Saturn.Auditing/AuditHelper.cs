using System.Collections.Generic;
using System.Linq;
using Audit.Core;
using ChangeTracking;
using FastMember;
using GoLive.Saturn.Data.Entities;

namespace GoLive.Saturn.Auditing
{
    public static class AuditHelper
    {
        public static T SetChangeLog<T>(this T item, AuditScope scope) where T : Entity
        {
            var changelog = item.CastToIChangeTrackable();
            if (changelog == null)
            {
                return item;
            }

            if (!changelog.IsChanged)
            {
                return changelog.GetCurrent();
            }

            var origItem = changelog.GetCurrent();
            var accessor = TypeAccessor.Create(origItem.GetType());

            var enumerable = accessor.GetMembers().Where(f => f.Type.IsGenericType);
            List<string> refProperties = new List<string>();

            foreach (var member in enumerable)
            {
                if (member.Type.GetGenericTypeDefinition() == typeof(List<>) && member.Type.GenericTypeArguments.FirstOrDefault()?.GetGenericTypeDefinition() == typeof(Ref<>))
                {
                    refProperties.Add(member.Name);
                    var accessorItem = accessor[origItem, member.Name] as dynamic;

                    foreach (var @ref in accessorItem)
                    {
                        @ref.Item = null;
                    }
                }
            }

            var changedList = new List<ChangeLog>();

            foreach (var changedProperty in changelog.ChangedProperties)
            {
                var changed = new ChangeLog();

                changed.Name = changedProperty;
                changed.Previous = changelog.GetOriginalValue(changedProperty);
                changed.Current = accessor[origItem, changedProperty];

                if (refProperties.Contains(changedProperty))
                {
                    var previous = Enumerable.Except(changed.Previous, changed.Current);
                    var current = Enumerable.Except(changed.Current, changed.Previous);

                    if (Enumerable.Any(previous) || Enumerable.Any(current))
                    {
                        changed.Previous = previous;
                        changed.Current = current;
                    }
                    else
                    {
                        continue;
                    }
                }

                changedList.Add(changed);
            }


            scope.Event.CustomFields.Add("diff", changedList);

            item = changelog.GetCurrent();

            scope.Event.CustomFields.Add("EntityId", item.Id);

            return item;
        }
    }
}