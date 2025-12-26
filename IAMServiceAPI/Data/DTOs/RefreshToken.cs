using IAMService.Data.Identities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public string UserId { get; set; }

    // Navigation property only to prevent going to db two times when using the generate access token function
    public ApplicationUserIdentity User { get; set; }

    public string Token { get; set; }

    public bool IsRevoked { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
}