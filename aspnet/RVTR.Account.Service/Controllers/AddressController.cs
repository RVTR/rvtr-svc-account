using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RVTR.Account.Domain.Interfaces;
using RVTR.Account.Domain.Models;
using RVTR.Account.Service.ResponseObjects;

namespace RVTR.Account.Service.Controllers
{
  /// <summary>
  /// Represents the _Address Component_ class
  /// </summary>
  [ApiController]
  [ApiVersion("0.0")]
  [EnableCors("Public")]
  [Route("rest/account/{version:apiVersion}/[controller]")]
  public class AddressController : ControllerBase
  {
    private readonly ILogger<AddressController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// The _Address Component_ constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="unitOfWork"></param>
    public AddressController(ILogger<AddressController> logger, IUnitOfWork unitOfWork)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Delete a user's address
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        await _unitOfWork.Address.DeleteAsync(id);
        await _unitOfWork.CommitAsync();

        return Ok(MessageObject.Success);
      }
      catch(Exception error)
      {
        _logger.LogError(error, error.Message);

        return NotFound(new ErrorObject($"Address with ID number {id} does not exist."));
      }
    }

    /// <summary>
    /// Get all addresses
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
      return Ok(await _unitOfWork.Address.SelectAsync());
    }

    /// <summary>
    /// Get a user's address with address ID number
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AddressModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
      var addressModel = (await _unitOfWork.Address.SelectAsync(e => e.EntityId == id)).FirstOrDefault();

      if (addressModel == null)
      {
        return NotFound(new ErrorObject($"Address with ID number {id} does not exist."));
      }

      return Ok(addressModel);
    }

    /// <summary>
    /// Add an address to an account
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Post(AddressModel address)
    {
      await _unitOfWork.Address.InsertAsync(address);
      await _unitOfWork.CommitAsync();

      return Accepted(address);
    }

    /// <summary>
    /// Update a user's address
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(AddressModel address)
    {
      try
      {
        _unitOfWork.Address.Update(address);

        await _unitOfWork.CommitAsync();

        return Accepted(address);
      }
      catch(Exception error)
      {
        _logger.LogError(error, error.Message);

        return NotFound(new ErrorObject($"Address with ID number {address.EntityId} does not exist."));
      }
    }
  }
}
