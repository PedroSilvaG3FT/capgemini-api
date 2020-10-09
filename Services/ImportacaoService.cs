using capgemini_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace basecs.Services
{
    public class ImportacaoService
    {
        private ContatoContexto _context;

        public ImportacaoService(ContatoContexto context)
        {
            _context = context;
        }

        public async Task<dynamic> ImportarExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("Arquivo não encontrado");
            }

            var _listaErros = new List<string> { };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream).ConfigureAwait(false);

                using (var package = new ExcelPackage(memoryStream))
                {
                    for (int i = 1; i <= package.Workbook.Worksheets.Count; i++)
                    {
                        var totalRows = package.Workbook.Worksheets[i].Dimension?.Rows;
                        var totalCollumns = package.Workbook.Worksheets[i].Dimension?.Columns;

                        if(totalRows == 1) {
                            _listaErros.Add("O Arquivo está vazio, preencha para continuar");
                        }

                        using (var context = this._context)
                        {
                            for (int row = 2; row <= totalRows.Value; row++)
                            {
                                Importacao importacao = new Importacao();
                                for (int col = 1; col <= totalCollumns.Value; col++)
                                {
                                    var infoLogDefault = $"(ln:{row}; col:{col}) ";

                                    var valueField = package.Workbook.Worksheets[i].Cells[row, col].Text.ToString();
                                    if (string.IsNullOrEmpty(valueField))
                                    {
                                        _listaErros.Add(infoLogDefault + "Campo sem valor (Preencha todos os campos)");
                                        return _listaErros.ToArray();
                                    }

                                    if (col == 1)
                                    {
                                        var dateFieldString = package.Workbook.Worksheets[i].Cells[row, col].Text.ToString();
                                        DateTime dateField = Convert.ToDateTime(dateFieldString);
                                        DateTime currentDate = DateTime.Now;

                                        if (dateField <= currentDate)
                                        {
                                            _listaErros.Add(infoLogDefault + "campo data de entrega não pode ser menor ou igual que o dia atual.");
                                        }
                                        importacao.DataEntrega = dateField;
                                    }

                                    if (col == 2)
                                    {
                                        var description = package.Workbook.Worksheets[i].Cells[row, col].Text.ToString();
                                        if (description.Length > 50)
                                        {
                                            _listaErros.Add(infoLogDefault + "Campo descrição deve ter no máximo de 50 caráteres. ");
                                        }

                                        importacao.NomeProduto = description;
                                    }

                                    if (col == 3)
                                    {
                                        var quantity = int.Parse(package.Workbook.Worksheets[i].Cells[row, col].Text);
                                        if (quantity <= 0)
                                        {
                                            _listaErros.Add(infoLogDefault + "Campo quantidade tem que ser maior que zero. ");
                                        }

                                        importacao.Quantidade = quantity;
                                    }

                                    if (col == 4)
                                    {
                                        var value = decimal.Parse(package.Workbook.Worksheets[i].Cells[row, col].Text);
                                        value = Decimal.Round(value, 2);
                                        if (value <= 0)
                                        {
                                            _listaErros.Add(infoLogDefault + "Campo valor unitário deve ser maior que zero");
                                        }

                                        importacao.ValorUnitario = value;
                                    }
                                }

                                try
                                {
                                    context.Importacao.Add(importacao);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }


                            if (_listaErros.ToArray().Length == 0)
                            {
                                await context.SaveChangesAsync();
                            }

                        }
                    }
                }
            }
            return _listaErros.ToArray();
        }

        public async Task<List<Importacao>> ReturnAllImportacoes()
        {
            try
            {
                List<Importacao> lstImportacoes = await this._context.Importacao.ToListAsync();

                return lstImportacoes;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar a busca por Veículo: " + ex.Message);
            }
        }

        public async Task<Importacao> FindById(int id)
        {
            try
            {
                return await this._context.Importacao.SingleOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao buscar o Veículo." + ex.Message);
            }
        }
    }
}
