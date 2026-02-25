using Interconn.common;
using Interconn.Models;
using Interconn.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Interconn.Services
{
    public class MemberService : IMemberService
    {
        private readonly InterconnDbContext _context;

        public MemberService(InterconnDbContext context)
        {
            _context = context;
        }

        public async Task<List<Member>> GetMembers(string? keyword)
        {
            //組成script準備送資料庫
            IQueryable<Member> MemberList = _context.Members;

            //確認有沒有關鍵字查詢 如果有就做篩選
            if (!string.IsNullOrEmpty(keyword))
            {
                MemberList = MemberList.Where(m => m.Name.Contains(keyword));
            }

            //送進資料庫查詢 傳回結果用List接收
            List<Member> result = await MemberList.ToListAsync();

            return result;
        }

        public async Task<Member> MemberDetail(int id)
        {
            Member result = await _context.Members
                                  .Where(m => m.MemberId == id)
                                  .FirstOrDefaultAsync();

            return result;
        }

        public async Task<ServiceResult> CreateMember(MemberCreateVM member)
        {
            try
            {
                List<string> result = await UploadFiles(member.Files, member.Name);

                Member entity = new Member()
                {
                    Name = member.Name,
                    Gender = member.Gender.ToString(),
                    BirthDate = member.BirthDate,
                    Bio = member.Bio,
                    UpdatedTime = DateTime.Now,
                    AvatarPath = result.FirstOrDefault()
                };

                _context.Members.Add(entity);
                await _context.SaveChangesAsync();

                return ServiceResult.Success("資料建立成功");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }

        public async Task<List<string>> UploadFiles(IEnumerable<IFormFile> files, string name)
        {
            int MaxFiles = 1; //限制照片張數
            string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".heic", ".gif" }; //可接受檔案類型
            long MaxFileSize = 15 * 1024 * 1024; // 限制檔案大小

            List<string> result = new List<string>();

            if (files == null || !files.Any())
            {
                return result;
            }

            if (files.Count() > MaxFiles)
            {
                throw new Exception($"最多只能上傳 {MaxFiles} 個檔案");
            }

            //上傳資料夾的路徑
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/members");

            //如果資料夾不存在就建立
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            foreach (var file in files)
            {
                //檔案大小檢查
                if (file.Length > MaxFileSize)
                {
                    throw new Exception($"檔案 {file.FileName} 太大，最大允許 {MaxFileSize / 1024 / 1024} MB");
                }

                //檔案副檔名檢查
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!AllowedExtensions.Contains(ext))
                {
                    throw new Exception($"檔案 {file.FileName} 格式不允許");
                }

                //這邊有小bug 我用Name當檔名 如果有人是相同名字檔案會被蓋過去
                var fileName = $"{name}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                //存相對路徑給前端使用
                result.Add($"/images/members/{fileName}");
            }

            return result;
        }

        public async Task<MemberEditVM> GetEditMember(int id)
        {
            //只查主鍵用FindAsync比較快 不用下Where 如果是空會回傳null
            Member member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return null;
            }

            //這邊要把資料庫取出來的member跟VM做映對
            //要取出來再做轉換 因為Gender轉成enum
            MemberEditVM result = new MemberEditVM
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Gender = Enum.Parse<GenderType>(member.Gender),
                BirthDate = member.BirthDate,
                Bio = member.Bio,
                AvatarPath = member.AvatarPath
            };

            return result;
        }

        public async Task<ServiceResult> PostEditMember(int id, MemberEditVM member)
        {
            Member update = await _context.Members.FindAsync(id);

            if (update == null)
            {
                return ServiceResult.NotFoundResult("找不到資料");
            }

            update.Name = member.Name;
            update.Gender = member.Gender.ToString();
            update.BirthDate = member.BirthDate;
            update.Bio = member.Bio;
            update.UpdatedTime = DateTime.Now;

            if (member.Files != null)
            {
                List<string> result = await UploadFiles(member.Files, member.Name);

                update.AvatarPath = result.FirstOrDefault();
            }
            try
            {
                await _context.SaveChangesAsync();

                return ServiceResult.Success("資料更新成功");
            }
            catch (DbUpdateException dbex)
            {
                return ServiceResult.Fail("更新資料失敗" + dbex.Message);
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }

        }

        public async Task<Member> GetDeleteMember(int id)
        {
            Member result = await _context.Members.FindAsync(id);

            return result;
        }

        public async Task<ServiceResult> PostDeleteMember(int id)
        {
            Member member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return ServiceResult.NotFoundResult("找不到資料");
            }

            try
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();

                return ServiceResult.Success("資料刪除成功");
            }

            catch (DbUpdateException)
            {
                return ServiceResult.Fail("刪除資料失敗");
            }

        }
    }
}
