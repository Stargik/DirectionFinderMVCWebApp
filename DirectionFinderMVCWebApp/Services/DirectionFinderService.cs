using System;
using DirectionFinderMVCWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirectionFinderMVCWebApp.Services
{
	public class DirectionFinderService
	{
		public DirectionFinderService()
		{
		}

		public List<List<double>> GetCoefsA(List<Locator> locators)
		{
            List<List<double>> coefs = new List<List<double>>()
            {
                new List<double>{0, 0 },
                new List<double>{0, 0 }
            };
			for (int i = 0; i < locators.Count; i++)
			{
                var rad = locators[i].Angle * Math.PI / 180;
                coefs[0][0] -= 1;
                coefs[1][0] += Math.Tan(locators[i].Angle * Math.PI / 180);
                coefs[0][1] = coefs[1][0];
                coefs[1][1] -= Math.Pow(Math.Tan(locators[i].Angle * Math.PI / 180), 2);
            }
			return coefs;
		}
        public List<double> GetCoefsB(List<Locator> locators)
        {
            List<double> coefs = new List<double> { 0, 0 };
            for (int i = 0; i < locators.Count; i++)
            {
                coefs[0] -= locators[i].Y - Math.Tan(locators[i].Angle * Math.PI / 180) * locators[i].X;
                coefs[1] -= (locators[i].Y - Math.Tan(locators[i].Angle * Math.PI / 180) * locators[i].X) * Math.Tan(locators[i].Angle * Math.PI / 180);
            }
            return coefs;
        }
        public List<List<double>> GetCoefs(List<Locator> locators)
        {
            List<List<double>> coefs = new List<List<double>>()
            {
                new List<double>{0, 0, 0 },
                new List<double>{0, 0, 0 }
            };
            for (int i = 0; i < locators.Count; i++)
            {
                coefs[0][0] -= 1;
                coefs[1][0] += Math.Tan(locators[i].Angle * Math.PI / 180);
                coefs[0][1] = coefs[1][0];
                coefs[1][1] -= Math.Pow(Math.Tan(locators[i].Angle * Math.PI / 180), 2);
                coefs[0][2] -= (locators[i].Y - Math.Tan(locators[i].Angle * Math.PI / 180) * locators[i].X);
                coefs[1][2] += (locators[i].Y - Math.Tan(locators[i].Angle * Math.PI / 180) * locators[i].X) * (Math.Tan(locators[i].Angle * Math.PI / 180));
            }
            return coefs;
        }

        public Point GetTargetPoint(List<List<double>> coefs)
        {
            var div = coefs[0][0];
            var k = coefs[1][0];
            for (int i = 0; i < coefs[0].Count; i++)
            {
                coefs[0][i] = coefs[0][i] / div;
                coefs[1][i] = coefs[1][i] - k * coefs[0][i];
            }
            coefs[1][2] = coefs[1][2] / coefs[1][1];
            coefs[0][2] = coefs[0][2] - coefs[1][2] * coefs[0][1];

            Point point = new Point { X = coefs[1][2], Y = coefs[0][2] };
            return point;
        }
    }
}

