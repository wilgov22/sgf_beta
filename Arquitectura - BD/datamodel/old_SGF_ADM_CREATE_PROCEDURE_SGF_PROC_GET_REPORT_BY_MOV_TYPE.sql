USE SGF;
GO
IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'SGF_PROC_GET_REPORT_BY_MOV_TYPE' AND XTYPE='P')
	DROP PROCEDURE SGFADM.SGF_PROC_GET_REPORT_BY_MOV_TYPE;
GO
CREATE PROCEDURE SGFADM.SGF_PROC_GET_REPORT_BY_MOV_TYPE(@in_accountId int,	
												 @in_MOV_TYPE_DESC VARCHAR(30),	
												  @in_beginDate VARCHAR(30),
												 @in_endDate VARCHAR(30))
/*INPUT
	@in_accountId		- accountID wich movements related to
	@in_MOV_TYPE_DESC	- movement type description
	@in_beginDate		- initial datetime period
	@in_endDate			- end datetime period
Example: select * from SGF_PROC_GET_REPORT_BY_MOV_TYPE(1,'Receitas','2012-03-27','2012-05-27')
*/

as


select 
	M1.MOV_ID as MOV_ID,
	M1.ACCOUNT_ID AS ACCOUNT_ID,
	C1.DESCRIPTION AS CATEGORY_DESC,
	SC1.DESCRIPTION AS SUBCATEGORY_DESC,
	M1.AMOUNT AS AMOUNT,
	M1.DESCRIPTION AS MOVEMENT_DESC,
	M1.CREATED_DATE AS MOVEMENT_DATE
from
	SGF_T_ACCOUNT_MOVEMENT M1 left join SGF_R_SUB_CATEGORY SC1 on M1.SUB_CAT_ID=SC1.SUB_CAT_ID, 
	SGF_R_CATEGORY C1,
	SGF_R_MOVEMENT_TYPE MT1
WHERE 
	M1.CAT_ID=C1.CAT_ID
AND
	M1.ACCOUNT_ID IN (@in_accountId) AND
	M1.CREATED_DATE BETWEEN @in_beginDate AND @in_endDate AND
	M1.MOV_TYPE_ID=MT1.MOV_TYPE_ID AND
	UPPER(M1.DESCRIPTION) like '%' + UPPER(@in_MOV_TYPE_DESC) + '%'

GO
GRANT EXEC ON OBJECT::SGFADM.SGF_PROC_GET_REPORT_BY_MOV_TYPE TO sgf_app;