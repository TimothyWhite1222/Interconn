using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Interconn.Models;
using Interconn.Services;
using Interconn.Models.ViewModels;
using Interconn.common;

namespace Interconn.Controllers
{
    public class MembersController : Controller
    {
        private readonly InterconnDbContext _context;
        private readonly IMemberService _service;

        public MembersController(InterconnDbContext context, IMemberService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Members
        public async Task<IActionResult> Index([FromQuery]string? keyword)
        {
            List<Member> result = await _service.GetMembers(keyword);

            return View(result);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details([FromRoute]int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member result = await _service.MemberDetail(id.Value);
            
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] MemberCreateVM member)
        {
            if (!ModelState.IsValid)
            {
                return View(member);
            }
            
            ServiceResult result = await _service.CreateMember(member);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError( "Files", result.Message);
                return View(member);
            }

            TempData["Message"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit([FromRoute]int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MemberEditVM result = await _service.GetEditMember(id.Value);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: Members/Edit/5     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] MemberEditVM member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(member);
            }

            ServiceResult result = await _service.PostEditMember(id, member);

            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(member);
            }

            TempData["Message"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete([FromRoute]int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member result = await _service.GetDeleteMember(id.Value);
            
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            ServiceResult result = await _service.PostDeleteMember(id);

            if (result.IsSuccess)
            {
                TempData["Message"] = result.Message;
            }
            else
            {
                TempData["Message"] = result.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        //在處理併發情況的時候可能會用到,例如在編輯的時候有人刪除了資料
        //可以用這個方法去確認是還存在這筆資料
        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.MemberId == id);
        }
    }
}
