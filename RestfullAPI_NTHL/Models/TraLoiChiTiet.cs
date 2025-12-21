using RestfullAPI_NTHL.Models;
using System.ComponentModel.DataAnnotations;

public class TraLoiChiTiet
{
    [Key]
    public int MaTraLoi { get; set; }
    public int MaKetQua { get; set; }
    public int MaCauHoi { get; set; }
    public string TraLoi { get; set; }
    public bool DungSai { get; set; }

    public virtual KetQuaQuiz KetQua { get; set; }
    public virtual CauHoi CauHoi { get; set; }
}