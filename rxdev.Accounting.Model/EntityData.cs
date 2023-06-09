namespace rxdev.Accounting.Model;

public class EntityData
    : Entity
{
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public Attachment? Attachment { get; set; }
    public int AttachmentId { get; set; }
}