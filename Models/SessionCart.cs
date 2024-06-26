﻿using LegosWithAurora.Infrastructure;
using Microsoft.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LegosWithAurora.Models
{
    public class SessionCart:Cart
    {
        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.Session;

            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();

            cart.Session = session;

            return cart;
        }
        [JsonIgnore]
        public ISession? Session { get; set; }

        public override void AddItem(Product p, int quantity)
        {
            base.AddItem(p, quantity);
            Session?.SetJson("Cart", this);
        }

        public override void RemoveLine(Product proj)
        {
            base.RemoveLine(proj);
            Session?.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session?.Remove("Cart");
        }
    }
}
