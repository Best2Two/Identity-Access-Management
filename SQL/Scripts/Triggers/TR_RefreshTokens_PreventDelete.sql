CREATE TRIGGER TR_RefreshTokens_PreventDelete
ON RefreshTokens
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update IsRevoked to 1 (true) instead of deleting
    UPDATE rt
    SET rt.IsRevoked = 1,
        rt.RevokedAt = GETDATE()
    FROM RefreshTokens rt
    INNER JOIN deleted d ON rt.Id = d.Id;
    
END;
GO