using Interconn.common;
using Interconn.Models;
using Interconn.Models.ViewModels;

namespace Interconn.Services
{
    public interface IMemberService
    {
        Task<List<Member>> GetMembers(string? keyword);
        Task<Member> MemberDetail(int id);
        Task<ServiceResult> CreateMember(MemberCreateVM member);
        Task<MemberEditVM> GetEditMember(int id);
        Task<ServiceResult> PostEditMember(int id, MemberEditVM member);
        Task<Member> GetDeleteMember(int id);
        Task<ServiceResult> PostDeleteMember(int id);
        Task<List<string>> UploadFiles(IEnumerable<IFormFile> files, string name);
    }
}
