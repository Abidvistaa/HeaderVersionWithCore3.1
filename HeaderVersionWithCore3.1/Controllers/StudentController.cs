using APICustomHeaderVersioning.Data;
using APICustomHeaderVersioning.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeaderVersion
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public StudentController(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<Student>>> getStudent()
        {
            return await _db.Students.ToListAsync();
        }

        
        [HttpGet]
        [MapToApiVersion("1.1")]
        public ActionResult additionCityNumber()
        {
            var cityAdd = _db.Students.Sum(x => x.tuitionFee);
            return Ok(cityAdd);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> setStudent([FromBody] Student Student)
        {
            _db.Students.Add(Student);
            await _db.SaveChangesAsync();

            return CreatedAtAction("getStudent", new { id = Student.studentId }, Student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateStudent(int id, Student Student)
        {
            if (id != Student.studentId)
            {
                return BadRequest();
            }

            _db.Entry(Student).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool StudentExist(int id)
        {
            return _db.Students.Any(e => e.studentId == id);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteStudent(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

