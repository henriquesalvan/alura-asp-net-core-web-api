using System.Linq;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;

namespace Alura.ListaLeitura.WebApp.Api
{
    public class LivrosController : Controller
    {
        private readonly IRepository<Livro> _repository;

        public LivrosController(IRepository<Livro> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Recuperar(int id)
        {
            var model = _repository.Find(id);

            if (model == null)
            {
                return NotFound();
            }

            return Json(model.ToModel());
        }

        [HttpPost]
        public IActionResult Incluir(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repository.Incluir(livro);

                var uri = Url.Action("Recuperar", new {id = livro.Id});
                return Created(uri, livro);
            }

            return BadRequest();
        }

        [HttpPost]
        public IActionResult Alterar(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (livro.ImagemCapa == null)
                {
                    livro.ImagemCapa = _repository.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }

                _repository.Alterar(livro);
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        public IActionResult Remover(int id)
        {
            var model = _repository.Find(id);
            if (model == null)
            {
                return NotFound();
            }

            _repository.Excluir(model);
            return NoContent();
        }
    }
}