namespace Api.Business;

public static class DateTimeExtension
{
	public static int GetAge(this DateTime dateOfBirth)
	{
		int age = DateTime.Today.Year - dateOfBirth.Year;

		if (DateTime.Today < dateOfBirth.AddYears(age))
		{
			age--;
		}

		return age;
	}
}
