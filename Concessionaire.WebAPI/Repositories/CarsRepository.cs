using Concessionaire.WebAPI.Contexts;
using Concessionaire.WebAPI.Dto;
using Concessionaire.WebAPI.Entities;
using Concessionaire.WebAPI.Enums;
using Concessionaire.WebAPI.Requests;
using Concessionaire.WebAPI.Services;

namespace Concessionaire.WebAPI.Repositories
{
    public class CarsRepository : ICarsRepository
    {
        private readonly ConcessionaireContext context;
        private readonly IAzureStorageService azureStorageService;
        public CarsRepository(ConcessionaireContext context, IAzureStorageService azureStorageService)
        {
            this.context = context;
            this.azureStorageService = azureStorageService;
        }

        public async Task<Car> AddAsync(CarRequest request)
        {
            var car = new Car()
            {
                Brand = request.Brand,
                Model = request.Model,
                Year = request.Year
            };

            if(request.Image != null)
            {
                car.ImagePath = await this.azureStorageService.UploadAsync(request.Image, ContainerEnum.IMAGES);
            }

            if (request.TechnicalDataSheet != null)
            {
                car.TechnicalDataSheetPath = await this.azureStorageService.UploadAsync(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS);
            }

            this.context.Add(car);
            this.context.SaveChanges();

            return car;
        }

        public List<Car> GetAll()
        {
            return this.context.Cars.ToList();
        }

    

        public async Task<CarDto> GetById(int id)
        {
            var car= this.context.Cars.Find(id);
            //descargar el archivo de la url segun TechnicalDataSheetPath
            if (car == null)
            {
                return null;
            }
            var file = await this.azureStorageService.DownloadAsync(ContainerEnum.IMAGES, car.TechnicalDataSheetPath);
            

            var resultData= new CarDto()
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                ImagePath = car.ImagePath,
                TechnicalDataSheetPath = car.TechnicalDataSheetPath,
                ImageBase64 = file

            };


            return resultData;
        }

        public async Task RemoveByIdAsync(int id)
        {
            var car = this.context.Cars.Find(id);
            if(car != null)
            {
                if(!string.IsNullOrEmpty(car.ImagePath))
                {
                    await this.azureStorageService.DeleteAsync(ContainerEnum.IMAGES, car.ImagePath);
                }

                if (!string.IsNullOrEmpty(car.TechnicalDataSheetPath))
                {
                    await this.azureStorageService.DeleteAsync(ContainerEnum.DOCUMENTS, car.TechnicalDataSheetPath);
                }

                this.context.Remove(car);
                this.context.SaveChanges();
            }
        }

        public async Task<Car> UpdateAsync(int id, CarRequest request)
        {
            var car = this.context.Cars.Find(id);
            if (car != null)
            {
                car.Brand = request.Brand;
                car.Model = request.Model;
                car.Year = request.Year;

                if (request.Image != null)
                {
                    car.ImagePath = await this.azureStorageService.UploadAsync(request.Image, ContainerEnum.IMAGES, car.ImagePath);
                }

                if (request.TechnicalDataSheet != null)
                {
                    car.TechnicalDataSheetPath = await this.azureStorageService.UploadAsync(request.TechnicalDataSheet, ContainerEnum.DOCUMENTS, car.TechnicalDataSheetPath);
                }

                this.context.Update(car);
                this.context.SaveChanges();
            }

            return car;
        }
    }

    public interface ICarsRepository
    {
        List<Car> GetAll();
        Task<CarDto> GetById(int id);
        Task<Car> AddAsync(CarRequest request);
        Task<Car> UpdateAsync(int id, CarRequest request);
        Task RemoveByIdAsync(int id);
    }
}
