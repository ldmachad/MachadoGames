using Machado_Games.Data;
using Machado_Games.Model;
using Microsoft.EntityFrameworkCore;

namespace Machado_Games.Service.Implements
{
    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _context;

        public ProdutoService(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Produto>> GetAll()
        {
             return await _context.Produtos
                .Include(p => p.Categoria)
                .ToListAsync();
        }

        public async Task<Produto?> GetById(long id)
        {
            try
            {
                var Produto = await _context.Produtos
                    .Include(p => p.Categoria)
                    .FirstAsync(i => i.Id == id);
                return Produto;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Produto>> GetByNomeOuConsole(string nome, string console)
        {
            return await _context.Produtos
                .Include(t => t.Categoria)
                .Where(produto => produto.Nome == nome || produto.Console == console)
                .ToListAsync();
        }

        public async Task<IEnumerable<Produto>> GetByPrecoIntervalo(decimal min, decimal max)
        {
            return await _context.Produtos
                .Include(t => t.Categoria)
                .Where(produto => produto.Preco >= min && produto.Preco <=max)
                .ToListAsync();            
        }

        public async Task<Produto?> Create(Produto produto)
        {
            if(produto.Categoria is not null)
            {
                var BuscaProduto = await _context.Categorias.FindAsync(produto.Categoria.Id);

                if(BuscaProduto is null)
                {
                    return null;
                }

                produto.Categoria = BuscaProduto;
            }
                       
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();

            return produto; 
        }

        public async Task<Produto?> Update(Produto produto)
        {
            var ProdutoUpdate = await _context.Produtos.FindAsync(produto.Id);

            if(ProdutoUpdate is null) 
            {
                return null;
            }

            if (produto.Categoria is not null)
            {
                var BuscaCategoria = await _context.Produtos.FindAsync(produto.Categoria.Id);

                if (BuscaCategoria is null)
                {
                    return null;
                }
            }

            produto.Categoria = produto.Categoria is not null ? _context.Categorias.FirstOrDefault(c => c.Id == produto.Categoria.Id) : null;

            _context.Entry(ProdutoUpdate).State = EntityState.Detached;
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return produto;
        }

        public async Task Delete(Produto produto)
        {
            _context.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }
}