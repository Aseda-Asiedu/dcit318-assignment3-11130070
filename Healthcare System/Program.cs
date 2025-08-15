using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthSystem
{
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new(items);

        public T? GetById(Func<T, bool> predicate)
            => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var found = items.FirstOrDefault(predicate);
            if (found is null) return false;
            items.Remove(found);
            return true;
        }
    }

    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id; Name = name; Age = age; Gender = gender;
        }

        public override string ToString() => $"[{Id}] {Name}, {Age}, {Gender}";
    }

    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
        }

        public override string ToString() => $"RNAQ#{Id} for Patient {PatientId}: {MedicationName} ({DateIssued:d})";
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Kwabena Yeboah", 30, "F"));
            _patientRepo.Add(new Patient(2, "Kwame Mensah", 42, "M"));
            _patientRepo.Add(new Patient(3, "Abena Fosuah", 25, "F"));
            _patientRepo.Add(new Patient(4, "Naa Amerley", 44, "M"));
            _patientRepo.Add(new Patient(5, "Sammy Tuga", 57, "M"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin 500mg", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Menthodex Cough Mixture 100mg", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Atorvastatin 20mg", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Ibuprofen 400mg", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(105, 3, "Metformin 500mg", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(106, 4, "Paracetamol 500mg", DateTime.Today.AddDays(-15)));
            _prescriptionRepo.Add(new Prescription(107, 5, "Wormplex Dewormer 1tab", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(108, 5, "Amoxyclav 500mg", DateTime.Today.AddDays(-7)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var rx in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(rx.PatientId))
                    _prescriptionMap[rx.PatientId] = new List<Prescription>();
                _prescriptionMap[rx.PatientId].Add(rx);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("All Patients:");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine("  " + p);
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var rxList = GetPrescriptionsByPatientId(id);
            if (rxList.Count == 0)
            {
                Console.WriteLine($"No prescriptions found for patient {id}.");
                return;
            }
            Console.WriteLine($"Prescriptions for patient {id}:");
            foreach (var rx in rxList)
                Console.WriteLine("  " + rx);
        }
    }

    public class Program
    {
        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            Console.WriteLine();
            int selectedPatientId = 5;
            app.PrintPrescriptionsForPatient(selectedPatientId);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
