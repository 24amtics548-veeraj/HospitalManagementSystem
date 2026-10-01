namespace HospitalManagementSystem.Models
{
    // Static class for extension methods[cite: 1]
    public static class DoctorExtensions
    {
        // Extension method to calculate total charge including Rs.100 service charge[cite: 1]
        public static decimal CalculateTotalCharge(this Doctor doctor)
        {
            // Adds fixed service fee of 100 to standard consultation fee[cite: 1]
            return doctor.ConsultationFee + 100;
        }
    }
}