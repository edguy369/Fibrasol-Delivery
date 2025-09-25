namespace Fibrasol_Delivery.Request;

public class RiderAssignationRequest
{
    public int RiderId { get; set; }
    public string ObjectState { get; set; } = default!; 
}
