USE SGF;
GO
IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'SGF_PROC_DASHBOARD' AND XTYPE='P')
	DROP PROCEDURE SGFADM.SGF_PROC_DASHBOARD;
GO

/*
USAGE: select * from SGF_PROC_DASHBOARD();
INPUTS: account_id
OUTPUTS: 
	Account_Id,
	TotalIncomeCurrentMonth,
	TotalExpensesCurrentMonth,
	TotalIncomePreviousMonth,
	TotalExpensesPreviousMonth,
	TotalIncomeCurrentYear,
	TotalExpensesCurrentYear
	
*/
CREATE PROCEDURE [SGFADM].[SGF_PROC_DASHBOARD](@in_accountId int) 
AS 
BEGIN

	SET DATEFORMAT ymd
	DECLARE @currentMonth VARCHAR(20)
	DECLARE @previousMonth VARCHAR(20)
	SET @currentMonth = (SELECT CONVERT(VARCHAR(8),GETDATE(),120) + '01')
	SET @previousMonth=  CONVERT(VARCHAR,DATEADD(m,-1,@currentMonth),111)

	SELECT	t0.Account_Id,
			isnull(t1.TotalIncomeCurrentMonth,0) TotalIncomeCurrentMonth,
			isnull(t2.TotalExpensesCurrentMonth,0) TotalExpensesCurrentMonth,
			isnull(t3.TotalIncomePreviousMonth,0) TotalIncomePreviousMonth,
			isnull(t4.TotalExpensesPreviousMonth,0) TotalExpensesPreviousMonth, 
			isnull(t5.TotalIncomeCurrentYear,0) TotalIncomeCurrentYear,
			isnull(t6.TotalExpensesCurrentYear,0) TotalExpensesCurrentYear
	FROM 
		(SELECT Account_Id FROM SGF_T_ACCOUNT WHERE ACCOUNT_ID IN (@in_accountId)) t0,
		(SELECT SUM(Amount) TotalIncomeCurrentMonth
				FROM SGF_T_ACCOUNT_MOVEMENT
					WHERE CREATED_DATE > = @currentMonth
						AND MOV_TYPE_ID IN (1) AND ACCOUNT_ID IN (@in_accountId)) t1,
		(SELECT SUM(Amount) TotalExpensesCurrentMonth 
				FROM SGF_T_ACCOUNT_MOVEMENT
					WHERE CREATED_DATE > = @currentMonth
						AND MOV_TYPE_ID IN (2) AND ACCOUNT_ID IN (@in_accountId))t2,
		(SELECT SUM(Amount) TotalIncomePreviousMonth
				FROM SGF_T_ACCOUNT_MOVEMENT
					WHERE CREATED_DATE > = @previousMonth AND CREATED_DATE < @currentMonth
						AND MOV_TYPE_ID IN (1) AND ACCOUNT_ID IN (@in_accountId)) t3,
		(SELECT SUM(Amount) TotalExpensesPreviousMonth 
				FROM SGF_T_ACCOUNT_MOVEMENT
					WHERE CREATED_DATE > = @previousMonth AND CREATED_DATE < @currentMonth
						AND MOV_TYPE_ID IN (2) AND ACCOUNT_ID IN (@in_accountId))t4,
		(SELECT SUM(Amount) TotalIncomeCurrentYear
				FROM SGF_T_ACCOUNT_MOVEMENT
					WHERE YEAR(CREATED_DATE) = YEAR(GETDATE())
						AND MOV_TYPE_ID IN (1) AND ACCOUNT_ID IN (@in_accountId))t5,
		(SELECT SUM(Amount) TotalExpensesCurrentYear
				FROM SGF_T_ACCOUNT_MOVEMENT
					WHERE YEAR(CREATED_DATE) = YEAR(GETDATE())
						AND MOV_TYPE_ID IN (2) AND ACCOUNT_ID IN (@in_accountId))t6
END
GO

GRANT EXEC ON OBJECT::SGFADM.SGF_PROC_DASHBOARD TO sgf_app;
GO
