using SPcomEF.Models;
using System;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;

namespace SPcomEF.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindEntities db;

        public ProductsController()
        {
            db = new NorthwindEntities();
        }

        [HttpGet, ActionName("GetProducts")]
        public async Task<ActionResult> Products()
        {
            IEnumerable<RelacaoDeProdutos> products = new List<RelacaoDeProdutos>();

            await Task.Run(() => products = db.RelacaoDeProdutos().ToList());

            return View("Products", products);
        }

        [HttpGet]
        public async Task<ActionResult> ProductsByCategory()
        {
            var category = 1; //Beverages
            return View(await Task.Run(() => db.ProdutosPorCategoria(category).ToList()));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Supplier);
            return View(await products.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await db.Products.FindAsync(id);
            if (product is null)
                return HttpNotFound();

            return View(product);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(db.Categories, nameof(Category.CategoryID), nameof(Category.CategoryName));
            ViewBag.Supplier = new SelectList(db.Suppliers, nameof(Supplier.SupplierID), nameof(Supplier.CompanyName));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Products.Add(product);
                    await db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Banco de Dados", "Não foi possível cadastrar o produto");
                    throw;
                }
            }

            ViewBag.Category = new SelectList(db.Categories, nameof(Category.CategoryID), nameof(Category.CategoryName));
            ViewBag.Supplier = new SelectList(db.Suppliers, nameof(Supplier.SupplierID), nameof(Supplier.CompanyName));

            return View(product);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await db.Products.FindAsync(id);
            if (product is null)
                return HttpNotFound();

            ViewBag.Category = new SelectList(db.Categories, nameof(Category.CategoryID), nameof(Category.CategoryName));
            ViewBag.Supplier = new SelectList(db.Suppliers, nameof(Supplier.SupplierID), nameof(Supplier.CompanyName));

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(product).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Banco de Dados", "Não foi possível cadastrar o usuário");
                    throw;
                }
            }

            ViewBag.Category = new SelectList(db.Categories, nameof(Category.CategoryID), nameof(Category.CategoryName));
            ViewBag.Supplier = new SelectList(db.Suppliers, nameof(Supplier.SupplierID), nameof(Supplier.CompanyName));

            return View(product);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await db.Products.FindAsync(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
