import { memo, useCallback, useRef, useState } from 'react';
import { Download, FileSpreadsheet, UploadCloud } from 'lucide-react';

interface UploadDropzoneProps {
  isUploading: boolean;
  onUpload: (file: File) => Promise<void>;
}

const ACCEPTED_EXTENSIONS = ['.csv', '.xlsx'];

const SAMPLE_CSV_CONTENT = [
  'Data;Valor;Descricao',
  '10/11/2026;4500.00;PIX CLIENTE DESIGN SYSTEM',
  '11/11/2026;-75.90;PAGAMENTO DAS MEI RECEITA FEDERAL',
  '12/11/2026;-120.50;COMPRA SUPRIMENTOS ESCRITORIO',
  '14/11/2026;-48.90;IFOOD PEDIDO SABADO',
  '15/11/2026;-32.00;UBER CORRIDA DOMINGO',
  '16/11/2026;1850.00;CONSULTORIA DESENVOLVIMENTO',
].join('\n');

export const UploadDropzone = memo(function UploadDropzone({
  isUploading,
  onUpload,
}: UploadDropzoneProps) {
  const inputRef = useRef<HTMLInputElement | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [localError, setLocalError] = useState<string | null>(null);

  const handleFile = useCallback(
    (file: File | undefined) => {
      if (!file) return;
      const lowerName = file.name.toLowerCase();
      const isValid = ACCEPTED_EXTENSIONS.some((ext) => lowerName.endsWith(ext));

      if (!isValid) {
        setLocalError('Formato inválido. Envie um arquivo .csv ou .xlsx.');
        return;
      }

      setLocalError(null);
      void onUpload(file);
    },
    [onUpload]
  );

  const handleDownloadSample = useCallback(() => {
    const blob = new Blob([SAMPLE_CSV_CONTENT], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'extrato-mei-exemplo.csv';
    link.click();
    URL.revokeObjectURL(url);
  }, []);

  return (
    <section className="panel upload-panel">
      <div className="panel-header">
        <div>
          <h2>Importar Extrato</h2>
          <p>Envie arquivos CSV (separado por `;`) ou XLSX para categorização automática.</p>
        </div>
        <button
          type="button"
          className="btn-secondary"
          onClick={handleDownloadSample}
        >
          <Download size={16} />
          Baixar CSV de Exemplo
        </button>
      </div>

      <div
        role="button"
        tabIndex={0}
        className={`dropzone ${isDragging ? 'dragging' : ''} ${isUploading ? 'uploading' : ''}`}
        onClick={() => inputRef.current?.click()}
        onKeyDown={(e) => {
          if (e.key === 'Enter' || e.key === ' ') inputRef.current?.click();
        }}
        onDragOver={(e) => {
          e.preventDefault();
          setIsDragging(true);
        }}
        onDragLeave={() => setIsDragging(false)}
        onDrop={(e) => {
          e.preventDefault();
          setIsDragging(false);
          handleFile(e.dataTransfer.files[0]);
        }}
      >
        <input
          ref={inputRef}
          type="file"
          accept=".csv,.xlsx"
          hidden
          onChange={(e) => {
            handleFile(e.target.files?.[0]);
            e.target.value = '';
          }}
        />
        {isUploading ? (
          <>
            <FileSpreadsheet className="spin" size={28} />
            <span>Processando e enriquecendo transações...</span>
          </>
        ) : (
          <>
            <UploadCloud size={28} />
            <span>Arraste seu extrato aqui ou clique para selecionar</span>
            <small>Suporta .csv e .xlsx • Deduplicação automática por hash</small>
          </>
        )}
      </div>

      {localError && <p className="inline-error">{localError}</p>}
    </section>
  );
});
