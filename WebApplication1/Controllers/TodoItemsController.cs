using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        #region DECLARE
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }
        #endregion

        /// <summary>
        /// Lấy item theo id
        /// </summary>
        /// <param name="id">id truyền vào</param>
        /// <returns>TodoItemDTO</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        /// <summary>
        /// Lấy toàn bộ item
        /// </summary>
        /// <returns>List item</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _context.TodoItems
            .Select(x => ItemToDTO(x))
            .ToListAsync();
        }

        /// <summary>
        /// Cập nhật item
        /// </summary>
        /// <param name="id">Id của iteam cần cập nhật</param>
        /// <param name="todoItemDTO">Dữ liệu item thay đổi</param>
        /// <returns>IActionResult</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="todoItemDTO">Item cần thêm mới</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoItemDTO)
        {
            var todoItem = new TodoItem
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, ItemToDTO(todoItem));
        }

        /// <summary>
        /// Xóa item theo id
        /// </summary>
        /// <param name="id">id item cần xóa</param>
        /// <returns>ActionResult</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        /// <summary>
        /// Xem item đã tồn tại hay chưa bằng id 
        /// </summary>
        /// <param name="id">id item cần kiểm tra</param>
        /// <returns>true=tồn tại, false = chưa tồn tại</returns>
        private bool TodoItemExists(long id) =>
         _context.TodoItems.Any(e => e.Id == id);

        /// <summary>
        /// Chuyển đổi todoitem thành todoitemDTO để gửi lên client
        /// </summary>
        /// <param name="todoItem">todoitem cần chuyển đổi</param>
        /// <returns>todoitemDTO</returns>
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
