ALTER TRIGGER trg_task
ON tbl_Task
AFTER UPDATE, INSERT, DELETE
AS
IF exists(SELECT * from tbl_Task) and exists (SELECT * from deleted)
BEGIN
    INSERT into dbo.tbl_Task_History(
	Task_ID,ShortDescription,FullDescription,CreatedDate,CreatorId,CreatorName,AssignedBy,AssignTo,AssignDate,ETC,TaskStatus,
	Priority,ClosureDate,Remarks,ExecutivesComments,R_Mod_Time,ModifiedBy)
	select
	i.ID,i.ShortDescription,i.FullDescription,i.CreatedDate,i.CreatorId,i.CreatorName,i.AssignedBy,i.AssignTo,i.AssignDate,i.ETC,
	i.TaskStatus,i.Priority,i.ClosureDate,i.Remarks,i.ExecutivesComments,GETDATE(),i.AssignedBy
	from inserted i;
END

IF exists (Select * from inserted) and not exists(Select * from deleted)
BEGIN
    INSERT into dbo.tbl_Task_History(
	Task_ID,ShortDescription,FullDescription,CreatedDate,CreatorId,CreatorName,AssignedBy,AssignTo,AssignDate,ETC,TaskStatus,
	Priority,ClosureDate,Remarks,ExecutivesComments,R_Mod_Time,ModifiedBy)
	select
	i.ID,i.ShortDescription,i.FullDescription,i.CreatedDate,i.CreatorId,i.CreatorName,i.AssignedBy,i.AssignTo,i.AssignDate,i.ETC,
	i.TaskStatus,i.Priority,i.ClosureDate,i.Remarks,i.ExecutivesComments,GETDATE(),i.AssignedBy
	from inserted i;
END

IF exists(select * from deleted) and not exists(Select * from inserted)
BEGIN 
	INSERT into dbo.tbl_Task_History(
	Task_ID,ShortDescription,FullDescription,CreatedDate,CreatorId,CreatorName,AssignedBy,AssignTo,AssignDate,ETC,TaskStatus,
	Priority,ClosureDate,Remarks,ExecutivesComments,R_Mod_Time,ModifiedBy)
	select
	i.ID,i.ShortDescription,i.FullDescription,i.CreatedDate,i.CreatorId,i.CreatorName,i.AssignedBy,i.AssignTo,i.AssignDate,i.ETC,
	i.TaskStatus,i.Priority,i.ClosureDate,i.Remarks,i.ExecutivesComments,GETDATE(),i.AssignedBy
	from deleted i;
END


