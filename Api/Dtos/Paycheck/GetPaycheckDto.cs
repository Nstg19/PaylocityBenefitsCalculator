namespace Api.Dtos.Paycheck;

public class GetPaycheckDto
{
	public int Id { get; set; }
	public int EmployeeId { get; set; }
	public decimal GrossPay { get; set; }
	public decimal Deductions { get; set; }
	public decimal NetPay { get; set; }
	public decimal YearToDate { get; set; }
	public int HoursWorked { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public DateTime PayDate { get; set; }
}
