using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyCompany.Domain.Entities;
using MyCompany.Domain.Repositories.Abstract;

namespace MyCompany.Domain.Repositories.EntityFramework
{
    public class EFTextFieldsRepository : ITextFieldsRepository // Реализация интерфейса
    {
        private readonly AppDbContext context; // Экземпляр для связи
        public EFTextFieldsRepository(AppDbContext context) // Через внедрение зависимостей связываем 
        {
            this.context = context;
        }

        public IQueryable<TextField> getTextFields()
        {
            return context.TextFields;
        }

        public TextField GetTextFieldById(Guid id)
        {
            return context.TextFields.FirstOrDefault(x => x.Id == id);
        }

        public TextField getTextFieldByCodeWords(string codeWord)
        {
            return context.TextFields.FirstOrDefault(x => x.CodeWord == codeWord);
        }

        public void SaveTextField(TextField entity)
        {
            if (entity.Id == default)
                context.Entry(entity).State = EntityState.Added;
            else
                context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void DeleteTextField(Guid id)
        {
            context.TextFields.Remove(new TextField() {Id = id});
            context.SaveChanges();
        }
    }
}
