using System.Threading.Tasks;
using In.ProjectEKA.HipService.Patient.Model;

namespace In.ProjectEKA.HipService.Patient
{
    public interface IPatientNotificationService
    {
        Task Perform(HipPatientStatusNotification hipPatientStatusNotification);
    }
}