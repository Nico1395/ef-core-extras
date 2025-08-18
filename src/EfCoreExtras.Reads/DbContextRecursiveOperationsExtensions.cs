//using EfCoreExtras.Keys;
//using Microsoft.EntityFrameworkCore;
//using System.Collections;
//using System.Reflection;

//namespace EfCoreExtras.Reads;

///// <summary>
///// Provides extension methods for performing recursive operations on entities in a <see cref="DbContext"/>, such as adding, updating,
///// and removing entities along with their related child entities.
///// </summary>
//public static class DbContextRecursiveOperationsExtensions
//{
//    /// <summary>
//    /// Adds the specified entity and all its related child entities to the <see cref="DbContext"/>.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="entity">The entity to add.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void AddRecursively<TEntity>(this DbContext context, TEntity entity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.Add(entity);

//        var children = new List<EntityChild>();
//        context.GetChildrenRecursively(children, entity, useChildAttribute);

//        foreach (var child in children.WhereIsToApplyChanges())
//            context.Add(child.Instance);

//        foreach (var child in children.WhereIsToBeIgnored())
//            context.Detach(child.Instance);
//    }

//    /// <summary>
//    /// Detaches and adds the specified entity and all its related child entities to the <see cref="DbContext"/>.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="entity">The entity to detach and add.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void DetachAndAddRecursively<TEntity>(this DbContext context, TEntity entity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.DetachAndAdd(entity);

//        var children = new List<EntityChild>();
//        context.GetChildrenRecursively(children, entity, useChildAttribute);

//        foreach (var child in children.WhereIsToApplyChanges())
//            context.DetachAndAdd(child.Instance);

//        foreach (var child in children.WhereIsToBeIgnored())
//            context.Detach(child.Instance);
//    }

//    /// <summary>
//    /// Updates the specified entity and all its related child entities in the <see cref="DbContext"/>.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="entity">The entity to update.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void UpdateRecursively<TEntity>(this DbContext context, TEntity entity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.Update(entity);

//        var children = new List<EntityChild>();
//        context.GetChildrenRecursively(children, entity, useChildAttribute);

//        foreach (var child in children.WhereIsToApplyChanges())
//            context.Update(child.Instance);

//        foreach (var child in children.WhereIsToBeIgnored())
//            context.Detach(child.Instance);
//    }

//    /// <summary>
//    /// Detaches and updates the specified entity and all its related child entities in the <see cref="DbContext"/>.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="entity">The entity to detach and update.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void DetachAndUpdateRecursively<TEntity>(this DbContext context, TEntity entity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.DetachAndUpdate(entity);

//        var children = new List<EntityChild>();
//        context.GetChildrenRecursively(children, entity, useChildAttribute);

//        foreach (var child in children.WhereIsToApplyChanges())
//            context.DetachAndUpdate(child.Instance);

//        foreach (var child in children.WhereIsToBeIgnored())
//            context.Detach(child.Instance);
//    }

//    /// <summary>
//    /// Sets the values of the existing entity to match the updated entity, and recursively updates or removes child entities as needed.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="existingEntity">The existing entity to update.</param>
//    /// <param name="updatedEntity">The updated entity with new values.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>, <paramref name="existingEntity"/>, or <paramref name="updatedEntity"/> is <see langword="null"/>.</exception>
//    public static void SetValuesRecursively<TEntity>(this DbContext context, TEntity existingEntity, TEntity updatedEntity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.SetValues(existingEntity, updatedEntity);
//        context.Update(existingEntity);

//        var childrenFromExisting = new List<EntityChild>();
//        context.GetChildrenRecursively(childrenFromExisting, existingEntity, useChildAttribute);

//        var childrenFromUpdated = new List<EntityChild>();
//        context.GetChildrenRecursively(childrenFromUpdated, updatedEntity, useChildAttribute);

//        // Remove children that are no longer present in the updated entity
//        foreach (var existingChild in childrenFromExisting)
//        {
//            // If that child is not supposed to be tracked, we will purposefully detach it so nothing will be done to it
//            if (existingChild.Mode == EntityChildMode.Ignore && useChildAttribute)
//            {
//                context.Detach(existingChild.Instance);
//                continue;
//            }

//            var existingKeyValues = context.KeyValues(existingChild.Instance);
//            if (!childrenFromUpdated.Any(u => context.KeyValuesEqual(u.Instance, existingKeyValues)))
//                context.Remove(existingChild.Instance);
//        }

//        // Check updated children. When a key matches the existing values are updated.
//        // Otherwise new children are added to the existing ones.
//        // These do not need to be detached before adding, since they cannot/should not be tracked already by the change tracker.
//        foreach (var updatedChild in childrenFromUpdated)
//        {
//            // If that child is not supposed to be tracked, we will purposefully detach it so nothing will be done to it
//            if (updatedChild.Mode == EntityChildMode.Ignore && useChildAttribute)
//            {
//                context.Detach(updatedChild.Instance);
//                continue;
//            }

//            var updatedKeyValues = context.KeyValues(updatedChild.Instance);
//            var existingChild = childrenFromExisting.FirstOrDefault(c => context.KeyValuesEqual(updatedKeyValues, c.Instance));
//            if (existingChild != null)
//            {
//                context.SetValues(existingChild.Instance, updatedChild.Instance);
//                context.Update(existingChild.Instance);
//            }
//            else
//            {
//                context.Add(updatedChild.Instance);
//            }
//        }
//    }

//    /// <summary>
//    /// Removes the specified entity and all its related child entities from the <see cref="DbContext"/>.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="entity">The entity to remove.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void RemoveRecursively<TEntity>(this DbContext context, TEntity entity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.Remove(entity);

//        var children = new List<EntityChild>();
//        context.GetChildrenRecursively(children, entity, useChildAttribute);

//        foreach (var child in children.WhereIsToApplyChanges())
//            context.Remove(child.Instance);

//        foreach (var child in children.WhereIsToBeIgnored())
//            context.Detach(child.Instance);
//    }

//    /// <summary>
//    /// Detaches and removes the specified entity and all its related child entities from the <see cref="DbContext"/>.
//    /// </summary>
//    /// <typeparam name="TEntity">The type of the entity.</typeparam>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="entity">The entity to detach and remove.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void DetachAndRemoveRecursively<TEntity>(this DbContext context, TEntity entity, bool useChildAttribute = true)
//        where TEntity : class
//    {
//        context.DetachAndRemove(entity);

//        var children = new List<EntityChild>();
//        context.GetChildrenRecursively(children, entity, useChildAttribute);

//        foreach (var child in children.WhereIsToApplyChanges())
//            context.DetachAndRemove(child.Instance);

//        foreach (var child in children.WhereIsToBeIgnored())
//            context.Detach(child.Instance);
//    }

//    /// <summary>
//    /// Recursively retrieves all child entities of the specified entity and adds them to the provided list.
//    /// </summary>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="children">The list to which child entities will be added.</param>
//    /// <param name="entity">The entity whose child entities are to be retrieved.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>, <paramref name="children"/>, or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void GetChildrenRecursively(this DbContext context, List<EntityChild> children, object entity, bool useChildAttribute)
//    {
//        context.GetChildrenRecursively(children, entity, entity.GetType(), useChildAttribute);
//    }

//    /// <summary>
//    /// Recursively retrieves all child entities of the specified entity and adds them to the provided list.
//    /// </summary>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="children">The list to which child entities will be added.</param>
//    /// <param name="entity">The entity whose child entities are to be retrieved.</param>
//    /// <param name="entityType">The type of the entity.</param>
//    /// <param name="useChildAttribute">Determines whether to use the <see cref="ChildAttribute"/> as a descriminator for children or not.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>, <paramref name="children"/>, <paramref name="entity"/>, or <paramref name="entityType"/> is <see langword="null"/>.</exception>
//    public static void GetChildrenRecursively(this DbContext context, List<EntityChild> children, object entity, Type entityType, bool useChildAttribute)
//    {
//        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
//        foreach (var property in properties)
//        {
//            if (!IsEntityChild(property.PropertyType))
//                continue;

//            var child = property.GetValue(entity);
//            if (child == null)
//                continue;

//            var flaggedAsChild = property.GetCustomAttribute<ChildAttribute>() != null;
//            if (child is IEnumerable childList && child is not string)
//            {
//                foreach (var listElement in childList)
//                {
//                    var listElementType = listElement.GetType();
//                    if (listElement == null || !IsEntityChild(listElementType))
//                        continue;

//                    if (!flaggedAsChild && useChildAttribute)
//                        children.Add(EntityChild.CreateIgnored(listElement));
//                    else
//                        children.Add(EntityChild.CreateToApplyChanges(listElement));

//                    context.GetChildrenRecursively(children, listElement, useChildAttribute);
//                }
//            }
//            else
//            {
//                if (!flaggedAsChild && useChildAttribute)
//                    children.Add(EntityChild.CreateIgnored(child));
//                else
//                    children.Add(EntityChild.CreateToApplyChanges(child));

//                context.GetChildrenRecursively(children, child, useChildAttribute);
//            }
//        }
//    }

//    /// <summary>
//    /// Recursively retrieves all child entities of the specified entity and adds them to the provided dictionary.
//    /// </summary>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="children">The dictionary to which child entities will be added.</param>
//    /// <param name="entity">The entity whose child entities are to be retrieved.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>, <paramref name="children"/>, or <paramref name="entity"/> is <see langword="null"/>.</exception>
//    public static void GetChildrenRecursively(this DbContext context, Dictionary<EntityKey, object> children, object entity)
//    {
//        context.GetChildrenRecursively(children, entity, entity.GetType());
//    }

//    /// <summary>
//    /// Recursively retrieves all child entities of the specified entity and adds them to the provided dictionary.
//    /// </summary>
//    /// <param name="context">The <see cref="DbContext"/> instance.</param>
//    /// <param name="children">The dictionary to which child entities will be added.</param>
//    /// <param name="entity">The entity whose child entities are to be retrieved.</param>
//    /// <param name="entityType">The type of the entity.</param>
//    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>, <paramref name="children"/>, <paramref name="entity"/>, or <paramref name="entityType"/> is <see langword="null"/>.</exception>
//    public static void GetChildrenRecursively(this DbContext context, Dictionary<EntityKey, object> children, object entity, Type entityType)
//    {
//        foreach (var property in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
//        {
//            if (!IsEntityChild(property.PropertyType))
//                continue;

//            var child = property.GetValue(entity);
//            if (child == null)
//                continue;

//            if (child is IEnumerable childList && child is not string)
//            {
//                foreach (var listElement in childList)
//                {
//                    var listElementType = listElement.GetType();
//                    if (listElement == null || !IsEntityChild(listElementType))
//                        continue;

//                    var key = EntityKey.FromValues(context.KeyValues(listElement));
//                    children[key] = listElement;
//                    context.GetChildrenRecursively(children, listElement, listElementType);
//                }
//            }
//            else
//            {
//                var key = EntityKey.FromValues(context.KeyValues(child));
//                children[key] = child;
//                context.GetChildrenRecursively(children, child, child.GetType());
//            }
//        }
//    }
//}
