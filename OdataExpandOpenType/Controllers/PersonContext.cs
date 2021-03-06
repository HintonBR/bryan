﻿namespace OdataExpandOpenType.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Web.OData.Builder;

    public class PersonContext : DbContext
    {
        public PersonContext()
        {
            this.Database.CreateIfNotExists();
            this.Configuration.LazyLoadingEnabled = false;
        }

        public IDbSet<Person> Persons { get; set; }

        public IDbSet<Widget> Widgets { get; set; }

        public IDbSet<PersonAttribute> PersonAttributes { get; set; }

        public IDbSet<WidgetAttribute> WidgetAttributes { get; set; }        
    }

    public class Attribute
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class PersonAttribute : Attribute
    {
        public virtual Person Person { get; set; }

        [ForeignKey("Person")]
        public int PersonId { get; set; }
    }

    public class WidgetAttribute : Attribute
    {
        public virtual Widget Widget { get; set; }

        [ForeignKey("Widget")]
        public int WidgetId { get; set; }
    }


    public class Person 
    {
        public Person()
        {
            this.Attributes = new HashSet<PersonAttribute>();
            this.Widgets = new HashSet<Widget>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Contained]
        public virtual ICollection<Widget> Widgets { get; set; }

        [Contained]
        public virtual ICollection<PersonAttribute> Attributes { get; }
    }


    [NotMapped]
    public class OpenPerson : Person
    {
        [NotMapped]
        public Dictionary<string, object> Properties { get; set; }

        public Person ToPerson()
        {
            var person = new Person() { Name = this.Name, DateOfBirth = this.DateOfBirth };
            foreach (var widget in this.Widgets)
            {
                var newWidget = new Widget { Name = widget.Name, Value = widget.Value };
                foreach (var widgetAttribute in widget.Attributes)
                {
                    newWidget.Attributes.Add(
                        new WidgetAttribute { Name = widgetAttribute.Name, Value = widgetAttribute.Value });
                }

                person.Widgets.Add(newWidget);

            }

            foreach (var property in this.Properties)
            {
                person.Attributes.Add(new PersonAttribute { Name = property.Key, Value = property.Value?.ToString() });
            }
            return person;
        }
    }

    public class Widget 
    {
        public Widget()
        {
            this.Attributes = new HashSet<WidgetAttribute>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public double Value { get; set; }

        [Contained]
        public virtual ICollection<WidgetAttribute> Attributes { get; }        
    }    
}
