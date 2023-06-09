namespace rxdev.Accounting.Model;

public class Attachment
    : Entity
{
    public string FileName { get; set; } = string.Empty;
    public EntityData? EntityData { get; set; }
}