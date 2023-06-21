﻿using EBook.Models;

namespace EBook.DataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository: IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
