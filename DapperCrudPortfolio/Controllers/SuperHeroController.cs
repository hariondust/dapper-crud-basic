using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrudPortfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAll()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await GetHeroes(connection);
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> Get(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SuperHero hero = await GetHero(heroId, connection);
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<SuperHero>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into superheroes (name, firstname, lastname, place) values (@Name, @FirstName, @LastName, @Place)", hero);
            return Ok(hero);
        }

        [HttpPut]
        public async Task<ActionResult<SuperHero>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update superheroes set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", hero);
            return Ok(await GetHero(hero.Id, connection));
        }

        [HttpDelete("{heroId}")]
        public async Task<ActionResult<SuperHero>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from superheroes where id = @Id", new { Id = heroId });
            return Ok(await GetHeroes(connection));
        }

        private static async Task<SuperHero> GetHero(int heroId, SqlConnection connection)
        {
            return await connection.QueryFirstAsync<SuperHero>("select * from superheroes where id = @Id",
                            new { Id = heroId });
        }

        private static async Task<IEnumerable<SuperHero>> GetHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("select * from superheroes");
        }
    }
}
