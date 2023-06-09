namespace rxdev.Accounting.Model;

public abstract class Entity
{
    public int Id { get; set; }
    public DateTime EntityCreationDate {  get; set; } = DateTime.Now;
    public DateTime EntityEditionDate { get; set; } = DateTime.Now;
}