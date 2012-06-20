USE SGF;
GO
IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'SGF_PROC_GET_REPORT_BY_CATEGORIES' AND XTYPE='P')
	DROP PROCEDURE SGFADM.SGF_PROC_GET_REPORT_BY_CATEGORIES;
GO
CREATE PROCEDURE SGFADM.SGF_PROC_GET_REPORT_BY_CATEGORIES(
												 @in_accountId INT,	
												 @in_CATEGORY_ID INT = NULL,	
											     @in_beginDate VARCHAR(30) = NULL,
												 @in_endDate VARCHAR(30) = NULL)
/*INPUT
	@in_accountId		- accountID wich movements related to
	@in_CATEGORY_ID		- category ID to filter
	@in_beginDate		- initial datetime period (by default no data consider)
	@in_endDate			- end datetime period (by default no data consider)
	
Example exec SGF_FUNC_GET_REPORT_BY_CATEGORIES 1,1,'2012-03-27','2012-05-27'
*/
AS
BEGIN

	DECLARE @sqlCmd   NVARCHAR(MAX)
	DECLARE @sqlWhere NVARCHAR(1000)

	SET @sqlCmd = ''
	SET @sqlWhere = ''
	
	-- WHERE CLAUSE -> INI
	IF @in_beginDate IS NOT NULL
	BEGIN
		SET @sqlWhere = @sqlWhere + ' AND M1.CREATED_DATE >= ''' + @in_beginDate + '' 
	END
	
	IF @in_endDate IS NOT NULL
	BEGIN
		SET @sqlWhere = @sqlWhere + ' AND M1.CREATED_DATE < ''' + @in_endDate + '' 
	END
	
	IF @in_CATEGORY_ID IS NOT NULL
	BEGIN
		SET @sqlWhere = @sqlWhere + ' AND M1.CAT_ID = ' + CONVERT(VARCHAR,@in_CATEGORY_ID)
	END
	-- WHERE CLAUSE -> END
	
	-- SELECT CLAUSE -> INI
	SET @sqlCmd = @sqlCmd + '
							SELECT M1.ACCOUNT_ID AS ACCOUNT_ID,
								M1.MOV_TYPE_ID AS MOV_TYPE_ID,
								MT1.DESCRIPTION AS MOV_TYPE_ID_DESC,
								M1.CAT_ID AS CATEGORY_ID,
								C1.DESCRIPTION AS CATEGORY_DESC,
								M1.SUB_CAT_ID AS SUB_CATEGORY_ID,
								SC1.DESCRIPTION AS SUBCATEGORY_DESC,
								ISNULL(SUM(M1.AMOUNT),0) AS TOTAL_AMOUNT
							FROM
								SGF_T_ACCOUNT_MOVEMENT M1 left join SGF_R_SUB_CATEGORY SC1 on M1.SUB_CAT_ID = SC1.SUB_CAT_ID, 
								SGF_R_CATEGORY C1,
								SGF_R_MOVEMENT_TYPE MT1
							WHERE 
								M1.CAT_ID=C1.CAT_ID
							AND M1.ACCOUNT_ID IN (' + CONVERT(VARCHAR,@in_accountId) + ') 
							AND M1.MOV_TYPE_ID = MT1.MOV_TYPE_ID 
							AND M1.CAT_ID = C1.CAT_ID '
	-- SELECT CLAUSE -> END
	
	SET @sqlCmd = @sqlCmd + @sqlWhere + ' GROUP BY  M1.ACCOUNT_ID, M1.MOV_TYPE_ID, MT1.DESCRIPTION, M1.CAT_ID, C1.DESCRIPTION, M1.SUB_CAT_ID, SC1.DESCRIPTION'
	
	PRINT @sqlCmd
	EXEC sp_executesql @sqlCmd

END
GO	

GO
GRANT EXEC ON OBJECT::SGFADM.SGF_PROC_GET_REPORT_BY_CATEGORIES TO sgf_app;