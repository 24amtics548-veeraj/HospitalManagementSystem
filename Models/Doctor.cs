namespace HospitalManagementSystem.Models
{
    // Model class representing a Doctor[cite: 1]
    public class Doctor
    {
        // Unique identifier for the doctor[cite: 1]
        public int DoctorId { get; set; }

        // Name of the doctor[cite: 1]
        public string DoctorName { get; set; } = string.Empty;

        // Area of specialization (e.g., Cardiology, Dermatology)[cite: 1]
        public string Specialization { get; set; } = string.Empty;

        // Total years of experience[cite: 1]
        public int Experience { get; set; }

        // Standard consultation fee[cite: 1]
        public decimal ConsultationFee { get; set; }

        // Contact number of the doctor
        public string ContactNumber { get; set; } = string.Empty;
    }

    // Model class to handle post data from external API[cite: 1]
    public class Post
    {
        // Unique post identifier[cite: 1]
        public int Id { get; set; }

        // Title of the post[cite: 1]
        public string Title { get; set; } = string.Empty;
    }
}