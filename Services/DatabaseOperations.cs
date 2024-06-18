using Kanbanify.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;

namespace Kanbanify.Services
{
    public class DatabaseOperations
    {

        // Table constants

        const string ProjectsTable = "Projects";

        const string StagesTable = "Stages";

        const string ItemsTable = "Items";

        const string TasksTable = "Tasks";

        const string UsersTable = "Users";

        // Fields

        private readonly IndexedDBManager _manager;

        // Constructor
        public DatabaseOperations(IndexedDBManager manager)
        {
            _manager = manager;
        }

        // Methods - public

        public async Task<bool> Create<T>(T data)
        {
            string table = GetTableName(typeof(T));

            try
            {
                var record = new StoreRecord<T>
                {
                    Storename = table,
                    Data = data
                };

                await _manager.AddRecord(record);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public async Task<(bool success, T data)> Read<T>(Guid id)
        {
            string table = GetTableName(typeof(T));

            bool success = false;
            T data = default;

            try
            {
                data = await _manager.GetRecordById<Guid, T>(table, id);
                success = (data != null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return (success, data);
        }

        public async Task<(bool, List<T> data)> ReadAll<T>()
        {
            string table = GetTableName(typeof(T));

            var data = new List<T>();
            bool success = false;

            try
            {
                data = await _manager.GetRecords<T>(table);
                success = (data != null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return (success, data);
        }

        public async Task<bool> Update<T>(T data)
        {
            string table = GetTableName(typeof(T));

            try
            {
                var record = new StoreRecord<T>
                {
                    Storename = table,
                    Data = data
                };

                await _manager.UpdateRecord<T>(record);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public async Task<bool> Update<T>(List<T> list)
        {
            string table = GetTableName(typeof(T));
            bool success = true;
            
            for (int i = 0; i < list.Count - 1; i++)
            {
                try
                {
                    var record = new StoreRecord<T>
                    {
                        Storename = table,
                        Data = list[i]
                    };

                    await _manager.UpdateRecord(record);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    success = false;
                }
            }
            return success;
        } 

        public async Task<bool> Delete<T>(Guid id)
        {
            string table = GetTableName(typeof(T));

            try
            {
                await _manager.DeleteRecord(table, id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(UserModel user)
        {
            try
            {
                // Cascading delete - delete projects, items, and tasks
                user.ProjectIds.ForEach(async x =>
                {
                    var (success, project) = await Read<ProjectModel>(x);

                    if (success)
                    {
                        await Delete(project);
                    }
                });

                // Delete the user itself
                await _manager.DeleteRecord(UsersTable, user.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(ProjectModel project)
        {
            try
            {
                // Cascading delete - delete project items, and item tasks
                project.ItemIds.ForEach(async x =>
                {
                    var (success, item) = await Read<ItemModel>(x);

                    if (success)
                    {
                        await Delete(item);
                    }
                });

                // Delete project stages
                project.StageIds.ForEach(async x => 
                    await Delete<StageModel>(x));

                // Detele the project itself
                await _manager.DeleteRecord(ProjectsTable, project.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(ItemModel item)
        {
            try
            {
                // Cascading the delete  - delete item tasks
                foreach (var taskId in item.TaskIds)
                {
                    await Delete<TaskModel>(taskId);
                }

                // Delete the item itself
                await _manager.DeleteRecord(ItemsTable, item.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(TaskModel task)
        {
            try
            {
                await _manager.DeleteRecord(TasksTable, task.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> Delete(StageModel stage)
        {
            try
            {
                await _manager.DeleteRecord(StagesTable, stage.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        // Methods - private

        private string GetTableName(Type t)
        {
            if (t == typeof(ItemModel))
                return ItemsTable;

            if (t == typeof(TaskModel))
                return TasksTable;

            if (t == typeof(ProjectModel))
                return ProjectsTable;

            if (t == typeof(StageModel))
                return StagesTable;

            return UsersTable;
        }


    }
}
