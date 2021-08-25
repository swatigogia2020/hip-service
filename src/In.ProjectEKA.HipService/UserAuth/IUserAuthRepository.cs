using System;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.UserAuth.Model;
using Optional;

namespace In.ProjectEKA.HipService.UserAuth
{
    public interface IUserAuthRepository
    {
        Task<Option<AuthConfirm>> Add(AuthConfirm authConfirm);
        Task<Option<AuthConfirm>> Get(string healthId);
        bool Update(AuthConfirm authConfirm);
        Task<Tuple<string, Exception>> GetAccessToken(string healthId);
        Task<Option<NdhmDemographics>> AddDemographics(NdhmDemographics ndhmDemographics);
        Task<Option<NdhmDemographics>> GetDemographics(string healthId);
    }
}